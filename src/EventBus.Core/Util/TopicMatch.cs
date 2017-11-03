namespace EventBus.Core.Util
{
    public static class TopicMatch
    {
        // 判断两个topic名字是否匹配
        public static bool IsTopicMatch(string topic, string partern)
        {
            var segments = topic.Split('.');
            var parternSegments = partern.Split('.');

            var sLen = segments.Length;
            var pLen = parternSegments.Length;

            var dp = new bool[sLen + 1][];
            for (var i = 0; i < sLen + 1; ++i)
            {
                dp[i] = new bool[pLen + 1];
            }

            dp[0][0] = true;
            for (var i = 1; i < sLen + 1; ++i)
            {
                dp[i][0] = false;
            }

            for (var j = 1; j < pLen + 1; j++)
            {
                if (parternSegments[j - 1] == "#")
                {
                    dp[0][j] = true;
                }
                else
                {
                    break;
                }
            }

            for (var i = 1; i < sLen + 1; ++i)
            {
                for (var j = 1; j < pLen + 1; ++j)
                {
                    if (parternSegments[j - 1] != "#")
                    {
                        dp[i][j] = dp[i - 1][j - 1] &&
                                   (segments[i - 1] == parternSegments[j - 1] || parternSegments[j - 1] == "*");
                    }
                    else
                    {
                        dp[i][j] = dp[i - 1][j] || dp[i][j - 1];
                    }
                }
            }

            return dp[sLen][pLen];
        }
    }
}
