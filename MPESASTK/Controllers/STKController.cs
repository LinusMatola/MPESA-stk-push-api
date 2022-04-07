using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MpesaLib;
using MPESASTK.Models;
using Newtonsoft.Json.Linq;
using Serilog;
using STKPushAPI.Models;
using System.Net;
using System.Text;

namespace MPESASTK.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class STKController : Controller
    {
        private readonly IMpesaClient _mpesaClient;
        private readonly mpesa_cred cred;
        public STKController(IMpesaClient mpesaClient, IOptions<mpesa_cred> optionCredSettings)
        {
            _mpesaClient = mpesaClient;
            cred = optionCredSettings.Value;
        }
     
        [HttpPost]
        public async Task<ActionResult<string>> CheckOut([FromBody] StkRequest stk)
        {
            string ConsumerKey = cred.ConsumerKey;
            string ConsumerSecret = cred.ConsumerSecret;
            string shortCode = cred.shortCode;
            string Passkey = cred.Passkey;
            string TillNumber = cred.shortCode;
            string CommandType = "CustomerPayBillOnline";
            string STKPushEndPoint = cred.STKPushEndPoint;
            string callback_endpoint = cred.callback_endpoint;

            string stkResultString = string.Empty;
            StkResponse stkResult = new StkResponse();
            //Async 
            try
            {
                var acces_token = await _mpesaClient.GetAuthTokenAsync(ConsumerKey, ConsumerSecret, RequestEndPoint.AuthToken);
                //return acces_token;
                HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(STKPushEndPoint);
                httpWebRequest2.Method = "POST";
                httpWebRequest2.Headers.Add("Authorization", "Bearer " + acces_token);
                httpWebRequest2.ContentType = "application/json";

                string Request_Date = DateTime.Now.ToString("yyyyMMddHHmmss");
                string trxDescription = "Payment " + stk.ExternalRef;
                byte[] bytes2 = Encoding.UTF8.GetBytes(shortCode + Passkey + Request_Date);
                string encPassword = Convert.ToBase64String(bytes2);
                string json_request_data = "{\n \"BusinessShortCode\":\"" + shortCode + "\",\n  \"Password\":\"" + encPassword + "\",\n  \"Timestamp\":\"" + Request_Date + "\",\n  \"TransactionType\":\"" + CommandType + "\",\n  \"Amount\":\"" + stk.Amt + "\",\n  \"PartyA\":\"" + stk.PhoneNumber + "\",\n  \"PartyB\":\"" + TillNumber + "\",\n  \"PhoneNumber\":\"" + stk.PhoneNumber + "\",\n  \"CallBackURL\":\"" + callback_endpoint + "\",\n  \"AccountReference\":\"" + stk.ExternalRef + "\",\n  \"TransactionDesc\":\"" + trxDescription + "\"\n \n}";
                Log.Logger.Information("STKPush Request: " + json_request_data);
                byte[] bytes3 = Encoding.UTF8.GetBytes(json_request_data);
                httpWebRequest2.ContentLength = bytes3.Length;
                Stream requestStream = httpWebRequest2.GetRequestStream();
                requestStream.Write(bytes3, 0, bytes3.Length);
                requestStream.Close();
                WebResponse response = httpWebRequest2.GetResponse();
                string statusDescription = ((HttpWebResponse)response).StatusDescription;
                requestStream = response.GetResponseStream();
                StreamReader streamReader2 = new StreamReader(requestStream);
                stkResultString = streamReader2.ReadToEnd();
                Log.Logger.Information("STKPush Result: " + stkResultString);
                JObject jsonResult = JObject.Parse(stkResultString);

                stkResult.ExternalRef = stk.ExternalRef;
                stkResult.MerchantID = jsonResult["MerchantRequestID"].ToString();
                stkResult.CheckoutRequestID = jsonResult["CheckoutRequestID"].ToString();
                stkResult.ResponseCode = jsonResult["ResponseCode"].ToString();
                stkResult.ResponseDescription = jsonResult["ResponseDescription"].ToString();
                stkResult.CustomerMessage = jsonResult["CustomerMessage"].ToString();
                //_context.Add(stkResult);
                //_context.SaveChangesAsync().Wait();
                streamReader2.Close();
                requestStream.Close();
                response.Close();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(stkResult);
             
        }
    }
}
