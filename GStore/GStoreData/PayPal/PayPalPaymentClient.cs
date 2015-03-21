using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using GStoreData.Models;
using GStoreData.PayPal.Models;
using Newtonsoft.Json;

namespace GStoreData.PayPal
{

	//info
	// https://developer.paypal.com/docs/integration/web/web-checkout/

	/// <summary>
	/// PayPal payment client class
	/// </summary>
	public class PayPalPaymentClient
	{
		/// <summary>
		/// Note; this method will throw an exception back if an error is encountered in the PayPal api, be sure to catch exceptions
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="cart"></param>
		/// <param name="return_url">URL to send user to after they confirm their payment info. Example: (current server url)/Checkout/PaymentSuccess</param>
		/// <param name="cancel_url">URL to send user to if they Cancel their payment info. Example: (current server url)/Checkout/PaymentCanceled</param>
		/// <returns></returns>
		public PayPalPaymentData StartPayPalPayment(StoreFrontConfiguration storeFrontConfig, Cart cart, Uri return_url, Uri cancel_url)
		{
			if (return_url == null)
			{
				throw new ArgumentNullException("return_url");
			}
			if (cancel_url == null)
			{
				throw new ArgumentNullException("cancel_url");
			}
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}

			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFront.CurrentConfig");
			}

			if (!storeFrontConfig.PaymentMethod_PayPal_Enabled)
			{
				throw new ApplicationException("PayPal is not enabled for this store front. Update Store Front Configuration to set storeFrontConfig.PayPal_UsePayPal = true");
			}

			string client_Id = storeFrontConfig.PaymentMethod_PayPal_Client_Id;
			if (string.IsNullOrWhiteSpace(client_Id))
			{
				throw new ArgumentNullException("storeFrontConfig.PayPal_Client_Id");
			}

			string client_Secret = storeFrontConfig.PaymentMethod_PayPal_Client_Secret;
			if (string.IsNullOrWhiteSpace(client_Secret))
			{
				throw new ArgumentNullException("storeFrontConfig.PayPal_Client_Secret");
			}

			bool useSandbox = !storeFrontConfig.PaymentMethod_PayPal_UseLiveServer;

			PayPalItemData[] items = cart.CartItems.Select(ci => new PayPalItemData(ci.Quantity.ToString(), ci.Product.Name, (ci.LineTotal / ci.Quantity).ToString("N2"), ci.Product.UrlName)).ToArray();

			PayPalOAuthTokenData token;
			try
			{
				token = GetOAuthToken(client_Id, client_Secret, useSandbox);
			}
			catch(Exceptions.PayPalExceptionOAuthFailed)
			{
				throw;
			}
			catch (Exception ex)
			{
				string message = "Paypal OAuth token call failed with exception " + ex.Message + ". See inner exception for details";
				throw new Exceptions.PayPalExceptionOAuthFailed(useSandbox, message, null, ex);
			}

			PayPalPaymentData result;
			try
			{
				result = CreatePayPalPayment(token, cart.Total, "Your Order at " + storeFrontConfig.Name, items, return_url.ToString(), cancel_url.ToString(), useSandbox);
			}
			catch (Exceptions.PayPalExceptionCreatePaymentFailed)
			{
				throw;
			}
			catch (Exception ex)
			{
				string message = "Paypal Create Payment API call failed with exception " + ex.Message + ". See inner exception for details";
				throw new Exceptions.PayPalExceptionCreatePaymentFailed(useSandbox, message, null, ex);
			}

			return result;

		}

		/// <summary>
		/// Note; this method should be called after the order is confirmed, and submitted to process.
		/// This is one of the last steps before the order can be placed.
		/// If this fails, the user will be sent back to the payment info page, and cart.StatusEnteredPaymentInfo should be set to false in the calling method
		/// Note; this method will throw an exception back if an error is encountered in the PayPal api, be sure to catch exceptions
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="cart"></param>
		/// <param name="payer_id">Pay Pal payer_id from querystring on payment confirm return</param>
		/// <returns></returns>
		public PayPalPaymentData ExecutePayPalPayment(StoreFrontConfiguration storeFrontConfig, Cart cart)
		{
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (cart.CartPaymentInfo == null)
			{
				throw new ArgumentNullException("cart.PaymentInfo");
			}
			if (!cart.StatusPaymentInfoConfirmed)
			{
				throw new ApplicationException("cart.StatusEnteredPaymentInfo = false. Cannot execute payment until PayPal payer is confirmed with returnUrl.");
			}
			if (string.IsNullOrWhiteSpace(cart.CartPaymentInfo.PayPalPaymentId))
			{
				throw new ArgumentNullException("cart.PaymentInfo.PayPalPaymentId", "cart.PaymentInfo.PayPalPaymentId = null. Be sure PayPalPaymentId is being set before Buyer is sent to PayPal to start auth or payment.");
			}
			if (string.IsNullOrWhiteSpace(cart.CartPaymentInfo.PayPalReturnPayerId))
			{
				throw new ArgumentNullException("cart.CartPaymentInfo.PayPalReturnPayerId", "cart.CartPaymentInfo.PayPalReturnPayerId = null. Be sure PayPalPayerId is being set in return from PayPal to start payment.");
			}
			if (cart.CartPaymentInfo.PayPalReturnPaymentId.ToLower() != cart.CartPaymentInfo.PayPalPaymentId.ToLower())
			{
				throw new ArgumentOutOfRangeException("PayPalReturnPaymentId", "cart.CartPaymentInfo.PayPalReturnPayerId != cart.CartPaymentInfo.PayPalPaymentId. This appears to be a URL hack.");
			}

			string client_Id = storeFrontConfig.PaymentMethod_PayPal_Client_Id;
			if (string.IsNullOrWhiteSpace(client_Id))
			{
				throw new ArgumentNullException("storeFrontConfig.PayPal_Client_Id");
			}

			string client_Secret = storeFrontConfig.PaymentMethod_PayPal_Client_Secret;
			if (string.IsNullOrWhiteSpace(client_Secret))
			{
				throw new ArgumentNullException("storeFrontConfig.PayPal_Client_Secret");
			}

			bool useSandbox = !storeFrontConfig.PaymentMethod_PayPal_UseLiveServer;

			PayPalOAuthTokenData token;
			try
			{
				token = GetOAuthToken(client_Id, client_Secret, useSandbox);
			}
			catch (Exceptions.PayPalExceptionOAuthFailed)
			{
				throw;
			}
			catch (Exception ex)
			{
				string message = "Paypal OAuth token call failed with exception " + ex.Message + ". See inner exception for details";
				throw new Exceptions.PayPalExceptionOAuthFailed(useSandbox, message, null, ex);
			}

			PayPalPaymentData result;
			try
			{
				result = ExecutePayment(token, cart.CartPaymentInfo.PayPalPaymentId, cart.CartPaymentInfo.PayPalReturnPayerId, useSandbox);
			}
			catch (Exceptions.PayPalExceptionCreatePaymentFailed)
			{
				throw;
			}
			catch (Exception ex)
			{
				string message = "Paypal Executer Payment API call failed with exception " + ex.Message + ". See inner exception for details";
				throw new Exceptions.PayPalExceptionCreatePaymentFailed(useSandbox, message, null, ex);
			}

			return result;
		}

		protected PayPalOAuthTokenData GetOAuthToken(string client_Id, string client_Secret, bool useSandbox)
		{

			string authUserName = client_Id;
			string authPassword = client_Secret;

			if (string.IsNullOrWhiteSpace(client_Id))
			{
				throw new ArgumentNullException("client_Id");
			}
			if (string.IsNullOrWhiteSpace(client_Secret))
			{
				throw new ArgumentNullException("client_secret");
			}

			HttpClient httpClient = new HttpClient();
			httpClient.Timeout = new TimeSpan(0, 0, 30);
			Uri requestUri = null;
			if (useSandbox)
			{
				httpClient.BaseAddress = new Uri("https://api.sandbox.paypal.com/");
			}
			else
			{
				httpClient.BaseAddress = new Uri("https://api.paypal.com/");
			}
			requestUri = new Uri("v1/oauth2/token", UriKind.Relative);

			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
				"Basic", 
				Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", authUserName, authPassword))));

			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

			List<KeyValuePair<string, string>> postValues = new List<KeyValuePair<string, string>>();
			postValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
			HttpContent postContent = new FormUrlEncodedContent(postValues);

			HttpResponseMessage response;
			try
			{
				response = httpClient.PostAsync(requestUri, postContent).Result;
			}
			catch (Exception ex)
			{
				string message = "HTTP Client returned an error during PayPal OAuth API post: " + ex.Message + ". See inner exception for details.";
				throw new Exceptions.PayPalExceptionOAuthFailed(useSandbox, message, null, ex);
			}

			PayPalOAuthTokenData oauthToken = new PayPalOAuthTokenData();
			if (response.IsSuccessStatusCode)
			{
				//get token data
				string json = response.Content.ReadAsStringAsync().Result;
				oauthToken = JsonConvert.DeserializeObject<PayPalOAuthTokenData>(json);
				oauthToken.Json = json;
			}
			else
			{
				string message = "PayPal OAuth Token API call failed! \nError code: " + response.StatusCode + " " + response.ReasonPhrase + "\n" + response.ToString();
				throw new Exceptions.PayPalExceptionOAuthFailed(useSandbox, message, response, null);
			}

			return oauthToken;
		}

		public PayPalOAuthTokenData TestPayPalOAuthToken(string client_Id, string client_Secret, bool useSandbox)
		{
			return GetOAuthToken(client_Id, client_Secret, useSandbox);
		}

		protected PayPalPaymentData CreatePayPalPayment(PayPalOAuthTokenData token, decimal total, string description, PayPalItemData[] items, string return_url, string cancel_url, bool useSandbox)
		{
			if (string.IsNullOrEmpty(token.access_token))
			{
				throw new ArgumentNullException("token.access_token");
			}
			if (string.IsNullOrEmpty(return_url))
			{
				throw new ArgumentNullException("return_url");
			}
			if (string.IsNullOrEmpty(cancel_url))
			{
				throw new ArgumentNullException("cancel_url");
			}
			if (total < 0.01M)
			{
				throw new ArgumentOutOfRangeException("total", "total must be 1 cent or more; value invalid: " + total.ToString());
			}

			PayPalPaymentData requestData = new PayPalPaymentData();
			requestData.intent = "sale";
			requestData.redirect_urls.return_url = return_url;
			requestData.redirect_urls.cancel_url = cancel_url;
			requestData.payer.payment_method = "paypal";

			List<PayPalTransactionData> transactions = new List<PayPalTransactionData>();
			transactions.Add(new PayPalTransactionData(total, description, items));
			requestData.transactions = transactions.ToArray();

			string requestJson = JsonConvert.SerializeObject(requestData, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			StringContent postContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

			HttpClient httpClient = new HttpClient();
			httpClient.Timeout = new TimeSpan(0, 0, 30);
			Uri requestUri = null;
			if (useSandbox)
			{
				httpClient.BaseAddress = new Uri("https://api.sandbox.paypal.com/");
			}
			else
			{
				httpClient.BaseAddress = new Uri("https://api.paypal.com/");
			}
			requestUri = new Uri("v1/payments/payment", UriKind.Relative);

			HttpResponseMessage response;
			httpClient.DefaultRequestHeaders.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
			try
			{
				response = httpClient.PostAsync(requestUri, postContent).Result;
			}
			catch (Exception ex)
			{
				string message = "HTTP Client returned an error during PayPal Create Payment API post: " + ex.Message + ". See inner exception for details.";
				throw new Exceptions.PayPalExceptionCreatePaymentFailed(useSandbox, message, null, ex);
			}

			PayPalPaymentData paymentResponseData;
			if (response.IsSuccessStatusCode)
			{
				//get PayPal data
				string json = response.Content.ReadAsStringAsync().Result;
				try
				{
					paymentResponseData = JsonConvert.DeserializeObject<PayPalPaymentData>(json);
				}
				catch (Exception ex)
				{
					string message = "Error reading PayPal Create Payment API result! \nError code: " + response.StatusCode + " " + response.ReasonPhrase + "\n" + response.ToString() + " see inner exception for details.\nJSON Response Data: " + json;
					throw new Exceptions.PayPalExceptionCreatePaymentFailed(useSandbox, message, response, ex);
				}
				paymentResponseData.Json = json;
			}
			else
			{
				string message = "PayPal Create Payment API call failed! \nError code: " + response.StatusCode + " " + response.ReasonPhrase + "\n" + response.ToString();
				throw new Exceptions.PayPalExceptionCreatePaymentFailed(useSandbox, message, response, null);
			}

			return paymentResponseData;
		}

		public PayPalPaymentData TestExecutePayment(PayPalOAuthTokenData token, string payPalPaymentId, string payer_id, bool useSandbox)
		{
			return ExecutePayment(token, payPalPaymentId, payer_id, useSandbox);
		}

		protected PayPalPaymentData ExecutePayment(PayPalOAuthTokenData token, string payPalPaymentId, string payer_id, bool useSandbox)
		{
			if (string.IsNullOrEmpty(token.access_token))
			{
				throw new ArgumentNullException("token.access_token");
			}
			if (string.IsNullOrEmpty(payPalPaymentId))
			{
				throw new ArgumentNullException("payPalPaymentId");
			}
			if (string.IsNullOrEmpty(payer_id))
			{
				throw new ArgumentNullException("payer_id");
			}

			PayPalPaymentExecuteData executeData = new PayPalPaymentExecuteData(payer_id);
			string executeJson = JsonConvert.SerializeObject(executeData);
			StringContent postContent = new StringContent(executeJson, Encoding.UTF8, "application/json");

			HttpClient httpClient = new HttpClient();
			httpClient.Timeout = new TimeSpan(0, 0, 30);
			Uri requestUri = null;
			if (useSandbox)
			{
				httpClient.BaseAddress = new Uri("https://api.sandbox.paypal.com/");
			}
			else
			{
				httpClient.BaseAddress = new Uri("https://api.paypal.com/");
			}
			requestUri = new Uri("v1/payments/payment/" + payPalPaymentId + "/execute", UriKind.Relative);

			HttpResponseMessage response = new HttpResponseMessage();
			httpClient.DefaultRequestHeaders.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

			try
			{
				response = httpClient.PostAsync(requestUri, postContent).Result;
			}
			catch (Exception ex)
			{
				string message = "HTTP Client returned an error during PayPal Execute Payment API post: " + ex.Message + ". See inner exception for details.";
				throw new Exceptions.PayPalExceptionExecutePaymentFailed(useSandbox, message, null, ex);
			}

			PayPalPaymentData executePaymentResponse;
			if (response.IsSuccessStatusCode)
			{
				//get PayPal data
				string json = response.Content.ReadAsStringAsync().Result;
				try
				{
					executePaymentResponse = JsonConvert.DeserializeObject<PayPalPaymentData>(json);
				}
				catch (Exception ex)
				{
					string message = "Error reading PayPal Execute Payment API result! \nError code: " + response.StatusCode + " " + response.ReasonPhrase + "\n" + response.ToString() + " see inner exception for details.\nJSON Response Data: " + json;
					throw new Exceptions.PayPalExceptionExecutePaymentFailed(useSandbox, message, response, ex);
				}
				executePaymentResponse.Json = json;
			}
			else
			{
				string message = "PayPal Execute Payment API call failed! \nError code: " + response.StatusCode + " " + response.ReasonPhrase + "\n" + response.ToString();
				throw new Exceptions.PayPalExceptionExecutePaymentFailed(useSandbox, message, response, null);
			}

			return executePaymentResponse;
		}


		public PayPalPaymentData TestCreateOneItemPayment(PayPalOAuthTokenData token, decimal total, string description, string return_url, string cancel_url, bool useSandbox)
		{
			if (string.IsNullOrEmpty(token.access_token))
			{
				throw new ArgumentNullException("token.access_token");
			}
			if (string.IsNullOrEmpty(return_url))
			{
				throw new ArgumentNullException("return_url");
			}
			if (string.IsNullOrEmpty(cancel_url))
			{
				throw new ArgumentNullException("cancel_url");
			}
			if (total < 0.01M)
			{
				throw new ArgumentOutOfRangeException("total", "total must be 1 cent or more; value invalid: " + total.ToString());
			}

			PayPalPaymentData requestData = new PayPalPaymentData();
			requestData.intent = "sale";
			requestData.redirect_urls.return_url = return_url;
			requestData.redirect_urls.cancel_url = cancel_url;
			requestData.payer.payment_method = "paypal";

			List<PayPalItemData> transactionItems = new List<PayPalItemData>();
			transactionItems.Add(new PayPalItemData("1", description, total.ToString("N2"), "X12345"));

			List<PayPalTransactionData> transactions = new List<PayPalTransactionData>();
			transactions.Add(new PayPalTransactionData(total, description, transactionItems.ToArray()));

			requestData.transactions = transactions.ToArray();

			string requestJson = JsonConvert.SerializeObject(requestData, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			StringContent postContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

			HttpClient httpClient = new HttpClient();
			httpClient.Timeout = new TimeSpan(0, 0, 30);
			Uri requestUri = null;
			if (useSandbox)
			{
				httpClient.BaseAddress = new Uri("https://api.sandbox.paypal.com/");
			}
			else
			{
				httpClient.BaseAddress = new Uri("https://api.paypal.com/");
			}
			requestUri = new Uri("v1/payments/payment", UriKind.Relative);

			httpClient.DefaultRequestHeaders.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

			HttpResponseMessage response = httpClient.PostAsync(requestUri, postContent).Result;
			PayPalPaymentData paymentResponseData;
			if (response.IsSuccessStatusCode)
			{
				//get token data
				string json = response.Content.ReadAsStringAsync().Result;
				paymentResponseData = JsonConvert.DeserializeObject<PayPalPaymentData>(json);
				paymentResponseData.Json = json;
			}
			else
			{
				throw new ApplicationException("PayPal Create One Item Payment API call failed! \nError code: " + response.StatusCode + " " + response.ReasonPhrase + "\n" + response.ToString());
			}

			return paymentResponseData;
		}



	}
}
