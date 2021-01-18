using System.Threading;

namespace SimplyView
{
    public class CancellationManager
    {
        private CancellationTokenSource? _TokenSource;

        public void Cancel()
        {
            var tokenSource = Interlocked.Exchange(ref _TokenSource, null);
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
        }

        public CancellationToken GetNextToken()
        {
            var newTokenSource = new CancellationTokenSource();
            var tokenSource = Interlocked.Exchange(ref _TokenSource, newTokenSource);
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
            return newTokenSource.Token;
        }
    }
}