using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using VersionOne.APIClient;
using VersionOne.ServiceHost.Eventing;
using VersionOne.Profile;

namespace VersionOne.ServiceHost.Core.Services
{
	public class V1WriterServiceBase
	{
		private ICentral _central;
		protected XmlElement _config;
		protected IEventManager _eventmanager;

		protected virtual ICentral Central
		{
			get
			{
				if (_central == null)
				{
					V1Central c = new V1Central(_config["Settings"]);
					c.Validate();
					_central = c;
				}
				return _central;
			}
		}

		public virtual void Initialize (XmlElement config, IEventManager eventmanager, IProfile profile)
		{
			_config = config;
			_eventmanager = eventmanager;
		}

		protected struct NeededAssetType
		{
			public readonly string Name;
			public readonly string[] AttributeDefinitionNames;
			public NeededAssetType (string name, string[] attributedefinitionnames)
			{
				Name = name;
				AttributeDefinitionNames = attributedefinitionnames;
			}
		}

		protected void VerifyNeededMeta (NeededAssetType[] neededassettypes)
		{
			foreach (NeededAssetType neededAssetType in neededassettypes)
			{
				IAssetType assettype = Central.MetaModel.GetAssetType(neededAssetType.Name);
				foreach (string attributeDefinitionName in neededAssetType.AttributeDefinitionNames)
				{
					IAttributeDefinition attribdef = assettype.GetAttributeDefinition(attributeDefinitionName);
				}
			}
		}


	}
}
