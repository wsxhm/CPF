namespace CPF.Mac.CoreData
{
	public enum NSManagedObjectContextConcurrencyType : ulong
	{
		Confinement,
		PrivateQueue,
		MainQueue
	}
}
