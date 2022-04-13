using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace HttpStressTest
{
    public class TestCaseStep : TestCaseStepDefinition
    {
        private static Regex regex = new Regex("({(?<param>[a-zA-Z0-9_]+)})", RegexOptions.Multiline | RegexOptions.Compiled);
        public TestCaseStep(TestCaseStepDefinition definition)
        {
            //copy
            this.Id= definition.Id;
            this.Get= definition.Get;
            this.Post= definition.Post;
            this.Headers= definition.Headers;
            this.Body= definition.Body;
            this.ResponseRegex = definition.ResponseRegex;
            this.AllowedStatuses= definition.AllowedStatuses;

            var pList = new List<string>();
            if (!string.IsNullOrEmpty(Get))
            {
                pList.AddRange(regex.Matches(Get).Select(s => s.Groups["param"].Value));
            }
            if (!string.IsNullOrEmpty(Post))
            {
                pList.AddRange(regex.Matches(Post).Select(s => s.Groups["param"].Value));
            }
            if (!string.IsNullOrEmpty(Body))
            {
                pList.AddRange(regex.Matches(Body).Select(s => s.Groups["param"].Value));
            }
            
            pList.AddRange(Headers.SelectMany(h=> regex.Matches(h).Select(s => s.Groups["param"].Value)));
            Parameters = pList.Distinct().ToArray();
            if (!string.IsNullOrEmpty(ResponseRegex))
                responseRegex = new Regex(ResponseRegex, RegexOptions.Multiline | RegexOptions.Compiled);
        }
        private Regex? responseRegex;
        public HttpRequestMessage ConstructHttpMessage(Dictionary<string, string> testData)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = string.IsNullOrEmpty(Post) ? HttpMethod.Get : HttpMethod.Post;
            message.RequestUri = new Uri(message.Method ==HttpMethod.Get?Replace(Get,testData):Replace(Post, testData) );
            if (message.Method == HttpMethod.Post && Body!=null)
            {
                message.Content =  new StringContent(Replace(Body,testData)) ;
            }
            foreach(var header in Headers)
            {
                var head=header.Split(':', StringSplitOptions.TrimEntries);
                if (head[0].Equals("content-type", StringComparison.InvariantCultureIgnoreCase))
                {
                    message.Content.Headers.ContentType = new MediaTypeHeaderValue(Replace(head[1], testData));
                }
                else
                {
                    message.Headers.Add(head[0], Replace(head[1],testData));
                }
                
                   
            }
            
            return message;
        }
        public Dictionary<string,string> ParseFromResponse(string text)
        {
            Dictionary<string,string> result = new Dictionary<string,string>();
            foreach(Match match in responseRegex.Matches(text))
            {
                foreach(Group group in match.Groups)
                {
                    if (group.Success && group.Name.Length > 0)
                    {
                        result.Add(group.Name, group.Value);
                    }
                }
            }
            return result;
        }
        
        private string Replace(string original,Dictionary<string, string> testData)
        {
            return regex.Replace(original, match => testData[match.Groups["param"].Value]);
        }
        public string[] Parameters{get;}
    }
}