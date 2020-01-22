﻿using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.Generic;

namespace GetMerakiOrgsCmdlet
{
    [Cmdlet(VerbsCommon.Get, "merakiorgs")]
    //[OutputType(typeof(MerakiOrg))]
    public class GteMerakiOrgsCommand : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Token { get; set; }

        [Parameter(
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        public string name { get; set; }

        private static readonly HttpClient client = new HttpClient();

        private static async Task<List<MerakiOrg>> GetOrgs(string Token)
        {
            //Cmdlet.WriteVerbose("Setting HTTP headers");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Cisco-Meraki-API-Key", Token);
            //Cmdlet.WriteVerbose("Making HTTP GET");
            var streamTask = client.GetStreamAsync("https://dashboard.meraki.com/api/v0/organizations");
            //Cmdlet.WriteVerbose("Awaiting JSON deserialization");
            return await JsonSerializer.DeserializeAsync<List<MerakiOrg>>(await streamTask);
        }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
            WriteVerbose(Token);
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override async void ProcessRecord()
        {
            WriteVerbose("Entering Get Orgs call");
            List<MerakiOrg> orgs = await GetOrgs(Token);

            WriteVerbose("Entering foreach");
            foreach (MerakiOrg org in orgs)
            {
                WriteVerbose(org.name);

                WriteObject(org);
            }


            WriteVerbose("Exiting foreach");
        }

        // // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }

    public class MerakiOrg
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
}