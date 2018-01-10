// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.


namespace AnuChandy.Fluent.Service.REST.Model
{
    /// <summary>
    /// Represents response from BeginCreate api call.
    /// </summary>
    public class BeginCreateResponse
    {
        public string pollingUrl { get; set; }

        public BeginCreateResponse(string pollingUrl)
        {
            this.pollingUrl = pollingUrl;
        }
    }
}
