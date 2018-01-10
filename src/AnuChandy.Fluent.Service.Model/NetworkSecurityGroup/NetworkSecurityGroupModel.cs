// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.NetworkSecurityGroup
{
    public class NetworkSecurityGroupModel
        : CreatableGroupableModel<Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate, Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate, INetworkSecurityGroup>
    {
        [JsonProperty(PropertyName = "rules")]
        public Dictionary<String, Rule> Rules { get; set; }

        public override async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken)
        {
            await base.ValidateAndResolveResourceGroupAsync(azure, fluentRequestModel, propertyName, parentModel, cancellationToken);
            if (this.Rules != null)
            {
                int i = 0;
                foreach (var rule in Rules)
                {
                    rule.Value.Validate($"{propertyName}.rules[{i}]");
                    i++;
                }
            }
        }

        protected override ICreatable<INetworkSecurityGroup> ToCreatableIntern(IAzure azure)
        {
            var withRegion = azure.NetworkSecurityGroups.Define(DeriveName("nsg"));
            var withGroup = withRegion.WithRegion(this.Region);
            var withCreate = SetResourceGroupAndTags(withGroup);
            if (Rules != null)
            {
                foreach (var rule in Rules)
                {
                    rule.Value.ToFluentRule(rule.Key, withCreate);
                }
            }
            return withCreate;
        }
    }

    public class Rule
    {
        [JsonProperty(PropertyName = "allowInbound")]
        public Direction AllowInbound { get; set; }

        [JsonProperty(PropertyName = "allowOutbound")]
        public Direction AllowOutbound { get; set; }

        [JsonProperty(PropertyName = "denyInbound")]
        public Direction DenyInbound { get; set; }

        [JsonProperty(PropertyName = "denyOutbound")]
        public Direction DenyOutbound { get; set; }

        [JsonProperty(PropertyName = "priority")]
        public int? Priority { get; set; }

        [JsonProperty(PropertyName = "description")]
        public String Description { get; set; }

        public void Validate(String propertyName)
        {
            if (this.AllowInbound == null 
                && this.AllowOutbound == null 
                && this.DenyInbound == null
                && this.DenyOutbound == null)
            {
                throw new ArgumentException($"{propertyName} is specified then one of the property {propertyName}.allowInbound, {propertyName}.allowOutbound, {propertyName}.denyInbound or {propertyName}.denyOutbound must be specified");
            }

            List<Boolean> booleans = new List<bool>();
            booleans.Add(this.AllowInbound != null);
            booleans.Add(this.AllowOutbound != null);
            booleans.Add(this.DenyInbound != null);
            booleans.Add(this.DenyOutbound != null);
            if (booleans.Count(b => b) > 1)
            {
                throw new ArgumentException($"{propertyName} is specified,  {propertyName}.allowInBound, {propertyName}.allowOutbound, {propertyName}.denyInbound, {propertyName}.denyOutbound are mutually exclusive");
            }
        }

        public Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate ToFluentRule(String ruleName, Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate withCreate)
        {
            Microsoft.Azure.Management.Network.Fluent.NetworkSecurityRule.Definition.IWithAttach<Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate> withAttach = null;

            if (this.AllowInbound != null)
            {
                withAttach = this.AllowInbound.ToFluentRule(ruleName, withCreate);
            }
            else if (this.AllowOutbound != null)
            {
                withAttach = this.AllowOutbound.ToFluentRule(ruleName, withCreate);
            }
            else if (this.DenyInbound != null)
            {
                withAttach = this.DenyInbound.ToFluentRule(ruleName, withCreate);
            }
            else if (this.DenyOutbound != null)
            {
                withAttach = this.DenyOutbound.ToFluentRule(ruleName, withCreate);
            }
            if (withAttach != null)
            {
                if (this.Priority != null)
                {
                    withAttach.WithPriority(this.Priority.Value);
                }
                if (this.Description != null)
                {
                    withAttach.WithDescription(this.Description);
                }
            }
            return withAttach.Attach();
        }
    }

    public class Direction
    {
        [JsonProperty(PropertyName = "fromAddress")]
        public String FromAddress { get; set; }

        [JsonProperty(PropertyName = "fromPort")]
        public String FromPort { get; set; }

        [JsonProperty(PropertyName = "fromPortRange")]
        public String FromPortRange { get; set; }

        [JsonProperty(PropertyName = "toAddress")]
        public String ToAddress { get; set; }

        [JsonProperty(PropertyName = "toPort")]
        public String ToPort { get; set; }

        [JsonProperty(PropertyName = "toPortRange")]
        public String ToPortRange { get; set; }

        [JsonProperty(PropertyName = "protocol")]
        public String Protocol { get; set; }

        public Microsoft.Azure.Management.Network.Fluent.NetworkSecurityRule.Definition.IWithAttach<Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate> ToFluentRule(String ruleName, Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate withCreate)
        {
            this.SetDefaults();

            var next1 = withCreate.DefineRule(ruleName)
                .AllowInbound();
            Microsoft.Azure.Management.Network.Fluent.NetworkSecurityRule.Definition.IWithSourcePort<Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate> next2;
            if (this.FromAddress != null)
            {
                next2 = next1.FromAddress(this.FromAddress);
            }
            else
            {
                next2 = next1.FromAnyAddress();
            }

            Microsoft.Azure.Management.Network.Fluent.NetworkSecurityRule.Definition.IWithDestinationAddress<Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate> next3;
            if (this.FromPort != null)
            {
                if (this.FromPort == "*")
                {
                    next3 = next2.FromAnyPort();
                }
                else
                {
                    next3 = next2.FromPort(Int32.Parse(this.FromPort));
                }
            }
            else
            {
                next3 = next2.FromAnyPort();
            }

            Microsoft.Azure.Management.Network.Fluent.NetworkSecurityRule.Definition.IWithDestinationPort<Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate> next4;
            if (this.ToAddress != null)
            {
                next4 = next3.ToAddress(this.ToAddress);
            }
            else
            {
                next4 = next3.ToAnyAddress();
            }

            Microsoft.Azure.Management.Network.Fluent.NetworkSecurityRule.Definition.IWithProtocol<Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate> next5;
            if (this.ToPort != null)
            {
                if (this.ToPort == "*")
                {
                    next5 = next4.ToAnyPort();
                }
                else
                {
                    next5 = next4.ToPort(Int32.Parse(this.ToPort));
                }
            }
            else
            {
                next5 = next4.ToAnyPort();
            }

            Microsoft.Azure.Management.Network.Fluent.NetworkSecurityRule.Definition.IWithAttach<Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Definition.IWithCreate> next6;
            if (this.Protocol != null)
            {
                next6 = next5.WithProtocol(this.Protocol);
            }
            else
            {
                next6 = next5.WithAnyProtocol();
            }
            return next6;
        }

        private Direction SetDefaults()
        {
            if (this.FromAddress == null)
            {
                this.FromAddress = "*";
            }
            if (this.FromPort == null && this.FromPortRange == null)
            {
                this.FromPort = "*";
            }
            if (this.ToAddress == null)
            {
                this.ToAddress = "*";
            }
            if (this.ToPort == null && this.ToPortRange == null)
            {
                this.ToPort = "*";
            }
            return this;
        }
    }
}
