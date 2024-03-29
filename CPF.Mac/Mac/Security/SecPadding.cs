namespace CPF.Mac.Security
{
	public enum SecPadding
	{
		None = 0,
		PKCS1 = 1,
		OAEP = 2,
		PKCS1MD2 = 0x8000,
		PKCS1MD5 = 32769,
		PKCS1SHA1 = 32770,
		PKCS1SHA224 = 32771,
		PKCS1SHA256 = 32772,
		PKCS1SHA384 = 32773,
		PKCS1SHA512 = 32774
	}
}
