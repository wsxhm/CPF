namespace CPF.Mac.Security
{
	internal struct AuthorizationItemSet
	{
		public int count;

		public unsafe AuthorizationItem* ptrToAuthorization;
	}
}
