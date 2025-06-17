namespace Design_Patterns.Builder;

public class Student
{
	private string name;
	private string email;
	private string phoneNumber;
	
	private Student(){}

	public void getDetails()
	{
		Console.WriteLine($"Student: {name} has registered email: {email} and phone number: {phoneNumber}");
	}

	public class StudentBuilder
	{
		private readonly Student _student = new Student();

		public StudentBuilder setName(string name)
		{
			_student.name = name;
			return this;
		}
		
		public StudentBuilder setEmail(string email)
		{
			_student.email = email;
			return this;
		}
		
		public StudentBuilder setPhoneNumber(string phoneNumber)
		{
			_student.phoneNumber = phoneNumber;
			return this;
		}

		public Student build()
		{
			return _student;
		}
	}
}