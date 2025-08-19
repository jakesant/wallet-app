namespace Wallet.Gateway
{
    public class EcbGatewayException : Exception
    {
        public EcbGatewayException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
