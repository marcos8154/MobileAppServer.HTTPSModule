using Newtonsoft.Json;
using SocketAppServer.CoreServices;
using SocketAppServer.CoreServices.CoreServer;
using SocketAppServer.ManagedServices;
using SocketAppServerClient;
using System;
using System.Linq;
using System.Web.Http;

namespace SocketAppServer.Extensions.HttpServer
{
    public class HTTPController : ApiController
    {
        private Client GetClient()
        {
            IServiceManager manager = ServiceManager.GetInstance();
            ICoreServerService coreServer = manager.GetService<ICoreServerService>();
            ServerConfiguration config = coreServer.GetConfiguration();
            Client client = new Client("localhost", config.Port, config.ServerEncoding, config.BufferSize);
            return client;
        }

        [HttpGet]
        public IHttpActionResult HttpGet(string controllerName, string actionName,
            [FromBody] RequestParameter[] parameters)
        {
            try
            {
                var rb = RequestBody.Create(controllerName, actionName);
                if (parameters != null)
                    parameters.ToList().ForEach(p => rb.AddParameter(p.Name, p.Value));

                Client client = GetClient();
                client.SendRequest(rb);
                var result = client.GetResult();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult HttpPost([FromBody]RequestBody rb)
        {
            try
            {
                Client client = GetClient();
                client.SendRequest(rb);
                var result = client.GetResult();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
