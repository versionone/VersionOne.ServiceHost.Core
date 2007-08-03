using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace VersionOne.IIS
{
	public abstract class IISObject : IDisposable
	{
		private DirectoryEntry _entry;
		private IISObject _parent;
		public IISObject Parent { get { return _parent; } }
		protected string Path { get { return _entry.Path; } }
		public string Name { get { return _entry.Name; } }

		public IISObject(string path)
		{
			_entry = new DirectoryEntry(path);
		}

		public void Save()
		{
			_entry.CommitChanges();
		}

		private void Configure(IISObject parent)
		{
			_parent = parent;
		}

		protected T Create<T>(string name, string type) where T : IISObject
		{
			if (_entry.Children.Find(name, type) == null)
			{
				DirectoryEntry e = _entry.Children.Add(name, type);
				e.CommitChanges();
			}
			return GetChild<T>(name);
		}

		protected T GetProperty<T>(string name)
		{
			return (T)_entry.InvokeGet(name);
		}

		protected void SetProperty<T>(string name, T value)
		{
			_entry.InvokeSet(name, value);
		}

		protected T GetChild<T>(string name) where T : IISObject
		{
			string fullpath = Path + "/" + name;
			if (DirectoryEntry.Exists(fullpath))
				return GetNode<T>(fullpath);
			return null;
		}

		private T GetNode<T>(string fullpath) where T : IISObject
		{
			T v = (T)Activator.CreateInstance(typeof(T), new object[] { fullpath });
			v.Configure(this);
			return v;
		}

		protected IDictionary<string, T> GetChildren<T>(string type) where T : IISObject
		{
			IDictionary<string, T> results = new Dictionary<string, T>();

			foreach (DirectoryEntry child in _entry.Children)
				if (child.SchemaClassName == type)
					results.Add(child.Name, GetChild<T>(child.Name));

			return results;
		}

		public virtual void Dispose()
		{
			_entry.Close();
		}
	}

	public class IISComputer : IISObject
	{
		private IISWebService _webservice;
		public IISWebService WebService
		{
			get
			{
				if (_webservice == null)
					_webservice = GetChild<IISWebService>("W3SVC");
				return _webservice;
			}
		}

		public IISComputer() : this("localhost") { }
		public IISComputer(string servername) : base("IIS://" + servername) { }

		public override void Dispose()
		{
			if (_webservice != null)
				_webservice.Dispose();
			base.Dispose();
		}
	}

	public class IISWebService : IISObject
	{
		public IISWebService(string path) : base(path) { }

		public new IISComputer Parent { get { return (IISComputer)base.Parent; } }

		private IDictionary<string, IISWebServer> _webservers;
		private IDictionary<string, IISWebServer> WebServersDict
		{
			get
			{
				if (_webservers == null)
					_webservers = GetChildren<IISWebServer>("IIsWebServer");
				return _webservers;
			}
		}

		public ICollection<IISWebServer> WebServers { get { return WebServersDict.Values; } }

		private IISApplicationPools _apppools;
		public IISApplicationPools AppPools
		{
			get
			{
				if (_apppools == null)
					_apppools = GetChild<IISApplicationPools>("AppPools");
				return _apppools;
			}
		}

		public override void Dispose()
		{
			if (_webservers != null)
				foreach (IISWebServer server in _webservers.Values)
					server.Dispose();

			if (_apppools != null)
				_apppools.Dispose();

			base.Dispose();
		}
	}

	public class IISApplicationPools : IISObject
	{
		public IISApplicationPools(string path) : base(path) { }

		public new IISWebService Parent { get { return (IISWebService)base.Parent; } }

		private IDictionary<string, IISApplicationPool> _apppools;
		private IDictionary<string, IISApplicationPool> AppPoolsDict
		{
			get
			{
				if (_apppools == null)
					_apppools = GetChildren<IISApplicationPool>("IIsApplicationPool");
				return _apppools;
			}
		}

		public ICollection<IISApplicationPool> AppPools { get { return AppPoolsDict.Values; } }

		public IISApplicationPool AddApplicationPool(string name)
		{
			IISApplicationPool apppool = Create<IISApplicationPool>(name, "IIsApplicationPool");
			AppPoolsDict.Add(name, apppool);
			return apppool;
		}

		public override void Dispose()
		{
			if (_apppools != null)
				foreach (IISApplicationPool apppool in _apppools.Values)
					apppool.Dispose();

			base.Dispose();
		}
	}

	public class IISApplicationPool : IISObject
	{
		public IISApplicationPool(string path) : base(path) { }

		public new IISApplicationPools Parent { get { return (IISApplicationPools)base.Parent; } }

		public int PeriodicRestartMemory
		{
			get { return GetProperty<int>("PeriodicRestartMemory"); }
			set { SetProperty("PeriodicRestartMemory", value); }
		}
		public int PeriodicRestartPrivateMemory
		{
			get { return GetProperty<int>("PeriodicRestartPrivateMemory"); }
			set { SetProperty("PeriodicRestartPrivateMemory", value); }
		}
	}

	public class IISWebServer : IISObject
	{
		public IISWebServer(string path) : base(path) { }

		public new IISWebService Parent { get { return (IISWebService)base.Parent; } }

		private IDictionary<string, IISRootWebVirtualDir> _virtualdirs;
		private IDictionary<string, IISRootWebVirtualDir> VirtualDirsDict
		{
			get
			{
				if (_virtualdirs == null)
					_virtualdirs = GetChildren<IISRootWebVirtualDir>("IIsWebVirtualDir");
				return _virtualdirs;
			}
		}

		public string ServerBindings { get { return GetProperty<string>("ServerBindings"); } }

		public string HostName
		{
			get
			{
				string[] parts = ServerBindings.Split(':');
				string host = "localhost";
				if (parts[2] != string.Empty)
					host = parts[2];
				return host;
			}
		}

		public int Port
		{
			get
			{
				string[] parts = ServerBindings.Split(':');
				string port = "80";
				if (parts[1] != string.Empty)
					port = parts[1];
				return int.Parse(port);
			}
		}

		public string Url
		{
			get
			{
				string url = "http://" + HostName;
				if (Port != 80)
					url += ":" + Port;
				return url;
			}
		}

		public ICollection<IISRootWebVirtualDir> VirtualDirs { get { return VirtualDirsDict.Values; } }
	}

	public abstract class IISBaseWebVirtualDir : IISObject
	{
		public IISBaseWebVirtualDir(string path) : base(path) { }

		public abstract bool IsRoot { get; }

		private IDictionary<string, IISWebVirtualDir> _virtualdirs;
		private IDictionary<string, IISWebVirtualDir> VirtualDirsDict
		{
			get
			{
				if (_virtualdirs == null)
					_virtualdirs = GetChildren<IISWebVirtualDir>("IIsWebVirtualDir");
				return _virtualdirs;
			}
		}

		public ICollection<IISWebVirtualDir> VirtualDirs { get { return VirtualDirsDict.Values; } }

		public override void Dispose()
		{
			if (_virtualdirs != null)
				foreach (IISWebVirtualDir vdir in _virtualdirs.Values)
					vdir.Dispose();

			base.Dispose();
		}

		public string AppPoolID
		{
			get { return GetProperty<string>("AppPoolID"); }
			set { SetProperty("AppPoolID", value); }
		}

		public IISWebVirtualDir GetVirtualDir(string instancename)
		{
			IISWebVirtualDir instance;
			if (VirtualDirsDict.TryGetValue(instancename, out instance))
				return instance;
			return null;
		}
	}

	public class IISRootWebVirtualDir : IISBaseWebVirtualDir
	{
		public override bool IsRoot { get { return true; } }
		public new IISWebServer Parent { get { return (IISWebServer)base.Parent; } }
		public IISRootWebVirtualDir(string path) : base(path) { }
	}

	public class IISWebVirtualDir : IISBaseWebVirtualDir
	{
		public IISWebVirtualDir(string path) : base(path) { }
		public new IISBaseWebVirtualDir Parent { get { return (IISBaseWebVirtualDir)base.Parent; } }
		public override bool IsRoot { get { return false; } }
	}
}
