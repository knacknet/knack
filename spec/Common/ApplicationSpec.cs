using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

	[TestFixture]
	public class ApplicationSpec {

		// Note: .NET 3.5 and 4.0 should be able to easily pass a lambda to a new Application(req => return a response);
		//       We should probabaly make 3.5 and 4.0 specific test assemblies for testing out extensions / features that 
		//       are available or that we add on those versions of the CLR.

		[Test]
		public void Can_create_a_new_application_using_an_external_method() {
			Application app = new Application(new ApplicationResponder(AppImplementation));
			Assert.That(app.GetResponse(new RequestWriter("/foo/bar")).BodyText, Is.EqualTo("You called us with Uri: /foo/bar"));
			Assert.That(app.GetResponse(new RequestWriter("/?hi=there")).BodyText, Is.EqualTo("You called us with Uri: /?hi=there"));
		}

		public IResponse AppImplementation(IRequest request) {
			return new Response("You called us with Uri: " + request.Uri);
		}

		class App1 : Application, IApplication {
			public override IResponse Invoke(IRequest request) {
				return new Response().SetBody(string.Format("Invoked {0} {1}", request.Method, request.Uri));
			}
		}

		[Test]
		public void Can_make_an_application_class_easily_that_uses_an_internal_method() {
			IApplication theApp = new App1();

			Response response = Application.GetResponse(theApp, new RequestWriter("POST", "/hi"));
			Assert.That(response.BodyText, Is.EqualTo("Invoked POST /hi"));

			response = Application.GetResponse(theApp, new RequestWriter("PUT", "/dogs/rover"));
			Assert.That(response.BodyText, Is.EqualTo("Invoked PUT /dogs/rover"));
		}
	}
}
