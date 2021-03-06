using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using MSDev.Work.Models;
using MSDev.Work.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSDev.Work.Helpers
{
    public class TranslateTextHelper
    {

        static AzureAuthTokenHelper _tokenHelper;
        public TranslateTextHelper(string subscriptKey)
        {
            _tokenHelper = new AzureAuthTokenHelper(subscriptKey);
        }

        /// <summary>
        /// 文本翻译
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string TranslateText(string content)
        {
            string seperator = "<!--divider-->";

            if (content.Length > 5000)
            {
                //分解 content
                content = addSeperator(content, 1);
            }
            // 内部方法，添加分隔符
            string addSeperator(string str, int tagLevel)
            {
                if (tagLevel > 5) return str; //避免无限递归

                var result = "";
                var tag = $"</h{tagLevel}>";
                if (str.Contains(tag))
                {
                    str = str.Replace(tag, tag + seperator);
                    var contentParts = str.Split(tag);
                    foreach (var item in contentParts)
                    {
                        var row = item + tag;
                        if (row.Length >= 5000)
                        {
                            row = addSeperator(row, tagLevel + 1);
                        }
                        result += row;
                    }
                    return result;
                }
                else
                {
                    result = addSeperator(str, tagLevel + 1);
                }
                return result;
            }

            var contentArray = content.Split(seperator);
            string translation = "";//最后翻译结果
            foreach (var item in contentArray)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                translation += GetBingTranslateAsync(item).Result;
            }
            return translation;
        }

        /// <summary>
        /// 使用智能感知翻译API
        /// </summary>
        /// <param name="content"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public string GetTranslate(string content, string from = "en", string to = "zh-CHS")
        {
            string result = "";
            string authToken = _tokenHelper.GetAccessToken();
            string uri = "https://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + HttpUtility.UrlEncode(content) + "&from=" + from + "&to=" + to;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            try
            {
                using (WebResponse response = httpWebRequest.GetResponse())
                using (Stream stream = response.GetResponseStream())
                {
                    DataContractSerializer dcs = new DataContractSerializer(Type.GetType("System.String"));
                    result += (string)dcs.ReadObject(stream);
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return default;
        }

        /// <summary>
        /// 使用必应神经网络示例翻译
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<string> GetBingTranslateAsync(string content)
        {
            string result = "";
            if (content == null)
            {
                return default;
            }
            using (var hc = new HttpClient())
            {

                hc.DefaultRequestHeaders.TryAddWithoutValidation("Host", "translator.microsoft.com");
                hc.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://translator.microsoft.com");
                hc.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063");
                hc.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", "ARRAffinity=f7f232231c2b9c0e7e4afcab9a2f3fab5ef0e78b791ebc50e4da15307c913d5d; MicrosoftApplicationsTelemetryDeviceId=7c43ec8a-e2cb-f005-b2e2-ffbe9ef5e8d9; MicrosoftApplicationsTelemetryFirstLaunchTime=1507822065801; wfvt_3586249983=59e4b6ef039c9; dnn.lang_to=zh-Hans; MC1=GUID=ad720ce26f3246ff833089b645d2cf56&HASH=ad72&LV=201710&V=4&LU=1506928549365; A=I&I=AxUFAAAAAACHBwAAPvSszMBL/Ww8VySy32LIlg!!&V=4; _ga=GA1.2.1962624722.1506947626; MSFPC=ID=ad720ce26f3246ff833089b645d2cf56&CS=3&LV=201710&V=1; MUID=1F97F122760D6BAF2019FA2D77AC6A5F; ANON=A=BAD034877E3773F0C11EE3C0FFFFFFFF&E=1459&W=1; NAP=V=1.9&E=13ff&C=tohUgPXblNgwsnYsOoBPXpeInkRGs2vkYOyY3mB1im2w42ui2trzRg&W=1; AMCV_EA76ADE95776D2EC7F000101%40AdobeOrg=-894706358%7CMCIDTS%7C17443%7CMCMID%7C28946246135856923772890756032015701046%7CMCAAMLH-1507611531%7C11%7CMCAAMB-1507611532%7CcIBAx_aQzFEHcPoEv0GwcQ%7CMCCIDH%7C1491354629%7CMCOPTOUT-1507013931s%7CNONE%7CMCSYNCSOP%7C411-17450%7CvVersion%7C2.3.0; AAMC_mscom_0=AMSYNCSOP%7C411-17450; __CT_Data=gpv=3&apv_1006_www32=2&cpv_1006_www32=2&apv_32381_www07=1&cpv_32381_www07=1&rpv_32381_www07=1; MSCC=1507216411; WT_FPC=id=20c2f9d8419105d0f4c1507760937121:lv=1507767129544:ss=1507765356870; omniID=1507818537161_0295_9967_e7cc_7ee37f6f95c7; WRUIDAWS=1437764312555795; _gid=GA1.2.439788021.1508161259");

                //hc.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://www.bing.com/translator");

                var requestContent =
                    new BingTranslateRequest
                    {
                        Text = content,
                        SourceLanguage = "en",
                        TargetLanguage = "zh-Hans"
                    };
                var body = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");

                try
                {
                    var response = await hc.PostAsync("https://translator.microsoft.com/neural/api/translator/translate", body);
                    var resultString = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeObject<BingTranslateResponse>(resultString);
                    result = responseObject.ResultNMT;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + e.Source);
                    Console.WriteLine("content:" + content);
                    return content;
                }

            }
            return result;
        }
    }
}
