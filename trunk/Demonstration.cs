using System;
using System.Collections.Generic;

namespace DocumentationReflector.AcidTest
{
	/// <summary>
	/// Public class.
	/// </summary>
	public class PublicClass
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentationReflector.AcidTest.PublicClass"/> class.
		/// </summary>
		public PublicClass()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentationReflector.AcidTest.PublicClass"/> class.
		/// </summary>
		/// <param name='parameter'>
		/// Parameter.
		/// </param>
		protected PublicClass(object parameter)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentationReflector.AcidTest.PublicClass"/> class.
		/// </summary>
		/// <param name='parameter'>
		/// Parameter.
		/// </param>
		/// <param name='otherParameter'>
		/// Other parameter.
		/// </param>
		private PublicClass(object parameter, object otherParameter)
		{
		}
		
		/// <summary>
		/// Gets or sets the public property.
		/// </summary>
		/// <value>
		/// The public property.
		/// </value>
		public object PublicProperty { get; set; }
		
		/// <summary>
		/// Gets or sets the type of the public property of generic.
		/// </summary>
		/// <value>
		/// The type of the public property of generic.
		/// </value>
		public IEnumerable<object> PublicPropertyOfGenericType { get; set; }
		
		/// <summary>
		/// Gets or sets the protected property.
		/// </summary>
		/// <value>
		/// The protected property.
		/// </value>
		protected object ProtectedProperty { get; set; }
		
		/// <summary>
		/// Gets or sets the private property.
		/// </summary>
		/// <value>
		/// The private property.
		/// </value>
		private object PrivateProperty { get; set; }
		
		/// <summary>
		/// The public field.
		/// </summary>
		public object PublicField;
		
		/// <summary>
		/// The type of the public field of generic.
		/// </summary>
		public IEnumerable<object> PublicFieldOfGenericType;
		
		/// <summary>
		/// The protected field.
		/// </summary>
		protected object ProtectedField;
		
		/// <summary>
		/// The private field.
		/// </summary>
		private object PrivateField;
		
		/// <summary>
		/// Public delegate returns void.
		/// </summary>
		public delegate void PublicDelegateReturnsVoid(object parameter);
		
		/// <summary>
		/// Public delegate.
		/// </summary>
		public delegate object PublicDelegate();
		
		/// <summary>
		/// Protected delegate.
		/// </summary>
		protected delegate object ProtectedDelegate();
		
		/// <summary>
		/// Private delegate.
		/// </summary>
		private delegate object PrivateDelegate();
		
		/// <summary>
		/// Occurs when public event.
		/// </summary>
		public event EventHandler PublicEvent;
		
		/// <summary>
		/// Occurs when protected event.
		/// </summary>
		protected event EventHandler ProtectedEvent;
		
		/// <summary>
		/// Occurs when private event.
		/// </summary>
		private event EventHandler PrivateEvent;
		
		/// <summary>
		/// Publics the method returns void.
		/// </summary>
		/// <param name='parameter'>
		/// Parameter.
		/// </param>
		public void PublicMethodReturnsVoid(object parameter)
		{
		}
		
		/// <summary>
		/// Publics the method.
		/// </summary>
		/// <returns>
		/// The method.
		/// </returns>
		public object PublicMethod()
		{
			return null;
		}
		
		/// <summary>
		/// Protecteds the method.
		/// </summary>
		/// <returns>
		/// The method.
		/// </returns>
		protected object ProtectedMethod()
		{
			return null;
		}
		
		/// <summary>
		/// Privates the method.
		/// </summary>
		/// <returns>
		/// The method.
		/// </returns>
		private object PrivateMethod()
		{
			return null;
		}
	}
}

