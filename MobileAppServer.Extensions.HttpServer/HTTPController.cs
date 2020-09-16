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
        private ISocketClientConnection GetClient()
        {
            IServiceManager manager = ServiceManager.GetInstance();
            ICoreServerService coreServer = manager.GetService<ICoreServerService>();
            ServerConfiguration config = coreServer.GetConfiguration();
            ISocketClientConnection connection = SocketConnectionFactory
                .GetConnection(new SocketClientSettings("localhost", config.Port, config.ServerEncoding));
            return connection;
        }

        [HttpGet]
        public IHttpActionResult HttpGet(string controllerName, string actionName,
            [FromBody] RequestParameter[] parameters)
        {
            try
            {
                OperationResult result = ExecuteRequest(controllerName, actionName,
                    parameters);

                if (result.Status == 600)
                    return Ok(result);
                else
                    return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += $@"\n{ex.InnerException.Message}";
                return InternalServerError(new Exception(msg));
            }
        }

        [HttpPost]
        public IHttpActionResult HttpPost(string controllerName, string actionName,
            [FromBody] RequestParameter[] parameters)
        {
            try
            {
                OperationResult result = ExecuteRequest(controllerName, actionName,
                    parameters);

                if (result.Status == 600)
                    return Ok(result);
                else
                    return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += $@"\n{ex.InnerException.Message}";
                return InternalServerError(new Exception(msg));
            }
        }

        private OperationResult ExecuteRequest(string controllerName, string actionName,
            [FromBody] RequestParameter[] parameters)
        {
            var rb = RequestBody.Create(controllerName, actionName);
            if (parameters != null)
                parameters.ToList().ForEach(p => rb.AddParameter(p.Name, p.Value));

            using (ISocketClientConnection client = GetClient())
            {
                client.SendRequest(rb);
                var result = client.GetResult();
                return result;
            }
        }
    }
}
