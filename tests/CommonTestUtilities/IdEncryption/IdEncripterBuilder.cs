using Sqids;

namespace CommonTestUtilities.IdEncryption
{
    public class IdEncripterBuilder
    {
        public static SqidsEncoder<long> Build()
        {
            return new SqidsEncoder<long>(new()
            {
                MinLength = 3,
                Alphabet = "l6AS3KUGXtnOz4jhf28rvpEI5b1Q97CMd"
            });
        }
    }
}
