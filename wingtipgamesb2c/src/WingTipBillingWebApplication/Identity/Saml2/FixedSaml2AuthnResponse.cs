using System.Xml;
using ITfoxtec.Identity.Saml2;

namespace WingTipBillingWebApplication.Identity.Saml2
{
    public class FixedSaml2AuthnResponse : Saml2AuthnResponse
    {
        public FixedSaml2AuthnResponse(Saml2Configuration configuration)
            : base(configuration)
        {
        }

        protected override XmlElement GetAssertionElement()
        {
            var assertionElements = XmlDocument.DocumentElement.SelectNodes("//*[local-name()='Assertion']");

            if (assertionElements.Count == 0)
            {
                return null;
            }

            var assertionElement = assertionElements[0] as XmlElement;
            var assertionDocument = new XmlDocument();
            assertionDocument.LoadXml(assertionElement.OuterXml);
            return assertionDocument.DocumentElement;
        }
    }
}
