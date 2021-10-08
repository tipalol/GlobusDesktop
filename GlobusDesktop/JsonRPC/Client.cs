using System;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlobusDesktop.JsonRPC
{
    /// <summary>A JSON RPC Client for handling Requests and Responses</summary>
    public class Client : IDisposable
    {
        #region Fields

        protected int globalId;

        protected readonly Uri url;

        protected readonly WebClient webClient;

        #endregion

        #region Properties

        /// <summary>The headers sent with every request.</summary>
        public WebHeaderCollection Headers { get { return webClient.Headers; } }

        public delegate void RpcCompleteDelegate(int token, Response response);

        /// <summary>This is raised when the Async rpc call returns a response from the server.</summary>
        public event RpcCompleteDelegate RpcAsyncCompleted;

        #endregion

        #region Constructor
        public Client(string baseUrl, int timeout = 0)
        {
            url = new Uri(baseUrl);

            webClient = new SessionWebClient();
            webClient.Headers.Add("Content-Type", "application/json");
            webClient.UploadDataCompleted += WebClientOnUploadDataCompleted;
            if (timeout > 0)
            {
                ((SessionWebClient)webClient).Timeout = timeout;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>Create a new Request with parameters</summary>
        public Request NewRequest(string methodName, JToken parameters)
        {
            return new Request(Interlocked.Increment(ref globalId), methodName, parameters);
        }

        /// <summary>Create a new Request without parameters</summary>
        public Request NewRequest(string methodName)
        {
            return NewRequest(methodName, null);
        }

        public GenericResponse Rpc(Request request)
        {
            return Rpc<GenericResponse>(request);
        }

        /// <summary>Perform a remote procedure call</summary>
        public virtual TResponse Rpc<TResponse>(Request request) where TResponse : Response
        {
            var requestSerialized = JsonConvert.SerializeObject(request);
            var requestBinary = Encoding.UTF8.GetBytes(requestSerialized);
            byte[] resultBinary;
            lock (webClient)
            {
                resultBinary = webClient.UploadData(url, "POST", requestBinary);
            }

            var resultSerialized = Encoding.UTF8.GetString(resultBinary);
            var response = JsonConvert.DeserializeObject<TResponse>(resultSerialized);

            return response;
        }

        /// <summary>Perform an asynchronous remote procedure call</summary>
        /// <param name="request">The Json Request</param>
        /// <returns>A token to be used to identify your rpc when <c>OnRpcCompleted</c> (token == reuqest.Id)</returns>
        public int RpcAsync(Request request)
        {
            string requestSerialized = JsonConvert.SerializeObject(request);
            byte[] requestBinary = Encoding.UTF8.GetBytes(requestSerialized);
            lock (webClient)
            {
                webClient.UploadDataAsync(url, "POST", requestBinary, request.Id);
            }
            return request.Id;
        }

        #endregion

        #region Non-Public Methods

        private void WebClientOnUploadDataCompleted(object sender, UploadDataCompletedEventArgs uploadDataCompletedEventArgs)
        {
            int token = (int)uploadDataCompletedEventArgs.UserState;
            byte[] responseBinary = uploadDataCompletedEventArgs.Result;
            string responseSerialized = Encoding.UTF8.GetString(responseBinary);
            Response response = JsonConvert.DeserializeObject<Response>(responseSerialized);

            // fire event.
            if (RpcAsyncCompleted != null)
                RpcAsyncCompleted(token, response);
        }

        public void Dispose()
        {
            lock (webClient)
            {
                webClient.UploadDataCompleted -= WebClientOnUploadDataCompleted;
                webClient.Dispose();
            }
        }

        #endregion
    }
}