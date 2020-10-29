using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class FAQController : ApiController
    {
        public object Get()
        {
            object obj = new object();
            try
            {
                BusinessLogicLayer.ReadConfig rcKey = new BusinessLogicLayer.ReadConfig(System.Web.HttpContext.Current.Server);
                List<FAQ> FaqList = new List<FAQ>();
                string Img = rcKey.LOCALIMG_PATH + "Content/img/" + "faqimg.jpg";
                FAQ faq = new FAQ();
                faq.Questions = new List<Question>();
                Question que = new Question();
                faq.Heading = "Registration & Account Related";
                que.Id = 1;
                que.Ques = "How do I buy from eZeelo?";
                que.Answer = "Visit Our website at www.ezeelo.com You may also choose to download Mobile app from Google Play Store (for Android) or App Store (for Apple iOS).Select the city, where you belong to.Select the product or service that you wish to get at your doorsteps, and add to cart.If you are registered user, login with your eZeelo userid and password, else register/signup.Place order, and confirm it, with place of delivery and preferred Delivery time!Choose payment option (COD or Online Payment mode).You will receive a confirmation over email / SMS.Our Customer Support will get in touch, in case you need it.You can also place your order on phone by calling our customer care between 10:00 am – 8:00 pm at 9172221910";
                faq.Questions.Add(que);
                FaqList.Add(faq);

                FAQ faq1 = new FAQ();
                faq1.Questions = new List<Question>();
                Question que1_1 = new Question();
                faq1.Heading = "Delivery Related";
                que1_1.Id = 1;
                que1_1.Ques = "Where can I get my orders delivered?";
                que1_1.Answer = "We provide doorstep delivery at your address. You can advise us to deliver at any location of your choice i.e your home/office or any other place.";
                faq1.Questions.Add(que1_1);
                Question que1_2 = new Question();
                que1_2.Id = 2;
                que1_2.Ques = "When can I get my orders delivered?";
                que1_2.Answer = "We provide same day delivery. You can choose from different timeslots available for delivery at your convenient time.";
                faq1.Questions.Add(que1_2);
                Question que1_3 = new Question();
                que1_3.Id = 3;
                que1_3.Ques = "Does eZeelo  deliver in my area?";
                que1_3.Answer = "You can place order from anywhere for delivery at any of our present location.";
                faq1.Questions.Add(que1_3);
                Question que1_4 = new Question();
                que1_4.Id = 4;
                que1_4.Ques = "How much are the delivery charges?";
                que1_4.Answer = "Local deliveries are usually free of cost. Nominal charge may be applicable based on cart value.";
                faq1.Questions.Add(que1_4);
                Question que1_5 = new Question();
                que1_5.Id = 5;
                que1_5.Ques = "How can I check the status of Delivery of my order?";
                que1_5.Answer = "You may login to eZeelo or check from your Mobile app. Check the status of your order in the My Orders, Delivery Status.";
                faq1.Questions.Add(que1_5);
                FaqList.Add(faq1);

                FAQ faq2 = new FAQ();
                faq2.Questions = new List<Question>();
                Question que2_1 = new Question();
                faq2.Heading = "Payment Related";
                que2_1.Id = 1;
                que2_1.Ques = "How can I pay for orders placed?";
                que2_1.Answer = "eZeelo offers online payment option, through secured gateways as well as cash on delivery (COD).";
                faq2.Questions.Add(que2_1);
                Question que2_2 = new Question();
                que2_2.Id = 2;
                que2_2.Ques = "Is it safe to use my credit/debit card on eZeelo?";
                que2_2.Answer = "eZeelo is linked to secured gateways like PayU, CCAvenue ensuring encrypted transactions. All your transactions are safe and secure!";
                faq2.Questions.Add(que2_2);
                Question que2_3 = new Question();
                que2_3.Id = 3;
                que2_3.Ques = "Is the data provided to eZeelo secured?";
                que2_3.Answer = "Customer information is kept in secured and encrypted form, and we at eZeelo never share any of the personal information with any third party for any reason. Transactions in eZeelo is kept safe and secured, including the online transactions which happen through payment gateways are ensured in high levels of encryption.";
                faq2.Questions.Add(que2_3);
                FaqList.Add(faq2);

                FAQ faq3 = new FAQ();
                faq3.Questions = new List<Question>();
                Question que3_1 = new Question();
                faq3.Heading = "Cancellations & Returns Related";
                que3_1.Id = 1;
                que3_1.Ques = "What happens if order is placed using online payment mode but the order could not be completed? How can I get refund?";
                que3_1.Answer = "In case your online payment is rejected by the Payment Gateway or the order could not be completed and in case the payment got deducted from your Debit/Credit Card, rest assured, it will be transferred back to your account within seven business days.  In case you still face problem, write to us at payments@ezeelo.com or call our Customer Care at 9172221910.  We shall be more than happy to be at your service and support.";
                faq3.Questions.Add(que3_1);
                Question que3_2 = new Question();
                que3_2.Id = 2;
                que3_2.Ques = "What should I do if I want to return the product?";
                que3_2.Answer = "If your product is found tampered/ opened, you may return it before acceptance. If the product is incorrect, damaged, tampered, defective, then you may contact our Customer Support at eZeelo, for easy facilitation. In case no claim being raised within 48 hours, it shall be presumed that you have waived the right to raise a complaint and eZeelo shall not be obliged to entertain any claim / dispute thereafter in respect thereof. If any incorrect / counterfeit product is supplied by Merchant, then the product will be taken back after due diligence. In case of product being found defective after the seal is opened or consignment is accepted by the customer, it shall be governed by the guarantee/ warranty policy of the Merchant/OEM as the case may be.";
                faq3.Questions.Add(que3_2);
                Question que3_3 = new Question();
                que3_3.Id = 3;
                que3_3.Ques = "How can I cancel my order?";
                que3_3.Answer = "Should you decide to cancel the orders, you may do so by Login to www.ezeelo.com. Proceed to -> My Orders -> Select your order -> hit Cancel. Alternatively, you may choose the Cancel Order option from the App. In case you need any help, you may always call us for Customer Support at 9172221910.";
                faq3.Questions.Add(que3_3);
                FaqList.Add(faq3);

                string Note = string.Empty;//"*(Subject to availability of product in your area)";
                obj = new { Success = 1, Message = "Successfull", Img = Img, Note = Note, data = FaqList };
            }
            catch (Exception ex)
            {
                obj = new { Success = 0, Message = ex.Message, data = string.Empty };
            }
            return obj;
        }

        public class FAQ
        {
            public string Heading { get; set; }
            public List<Question> Questions { get; set; }

            //public string Heading1 { get; set; }
            //public string SubHeading1 { get; set; }
            //public string AnsSubHeading1 { get; set; }
            //public string Heading2 { get; set; }
            //public string SubHeading2_1 { get; set; }
            //public string AnsSubHeading2_1 { get; set; }
            //public string SubHeading2_2 { get; set; }
            //public string AnsSubHeading2_2 { get; set; }
            //public string SubHeading2_3 { get; set; }
            //public string AnsSubHeading2_3 { get; set; }
            //public string SubHeading2_4 { get; set; }
            //public string AnsSubHeading2_4 { get; set; }
            //public string SubHeading2_5 { get; set; }
            //public string AnsSubHeading2_5 { get; set; }
            //public string Heading3 { get; set; }
            //public string SubHeading3_1 { get; set; }
            //public string AnsSubHeading3_1 { get; set; }
            //public string SubHeading3_2 { get; set; }
            //public string AnsSubHeading3_2 { get; set; }
            //public string SubHeading3_3 { get; set; }
            //public string AnsSubHeading3_3 { get; set; }
            //public string Heading4 { get; set; }
            //public string SubHeading4_1 { get; set; }
            //public string AnsSubHeading4_1 { get; set; }
            //public string SubHeading4_2 { get; set; }
            //public string AnsSubHeading4_2 { get; set; }
            //public string SubHeading4_3 { get; set; }
            //public string AnsSubHeading4_3 { get; set; }
            //public string Note { get; set; }

        }
        public class Question
        {
            public int Id { get; set; }
            public string Ques { get; set; }
            public string Answer { get; set; }
        }
    }
}
