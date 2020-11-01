namespace Pertuk.Business.CustomIdentity
{
    internal sealed class CustomSecurityToken
    {
        private readonly byte[] _data;

        public CustomSecurityToken(byte[] data)
        {
            _data = (byte[])data.Clone();
        }

        internal byte[] GetDataNoClone()
        {
            return _data;
        }
    }
}