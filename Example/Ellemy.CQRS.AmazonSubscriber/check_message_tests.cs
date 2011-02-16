using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using NUnit.Framework;
using Attribute = Amazon.SQS.Model.Attribute;

namespace Ellemy.CQRS.AmazonSubscriber
{
    [TestFixture]
    public class check_message_tests
    {
        private AmazonSQSClient _client;
        private string _queueUrl;

        [SetUp]
        public void create_client()
        {
            _queueUrl = "https://queue.amazonaws.com/451419498740/Test_for_blog";
            _client = new AmazonSQSClient("AKIAIN2KJH4QJIUV7CGQ", "18ypN0y7SGA+L0XDVMHm9lBVmQ2oF2bdm7CGIijA");
        }
        [Test]
        public void create_queue()
        {
            var request = new CreateQueueRequest().WithQueueName("Test_for_blog");
            var response = _client.CreateQueue(request);
            Console.WriteLine(response.CreateQueueResult.QueueUrl);

        }
        [Test]
        public void subscribe_to_topic()
        {
            var getArnRequest = new GetQueueAttributesRequest().WithQueueUrl(_queueUrl).WithAttributeName("QueueArn");
            var clientArn = _client.GetQueueAttributes(getArnRequest).GetQueueAttributesResult.Attribute[0].Value;

            var sns = Amazon.AWSClientFactory.CreateAmazonSNSClient("AKIAIN2KJH4QJIUV7CGQ",
                                                                    "18ypN0y7SGA+L0XDVMHm9lBVmQ2oF2bdm7CGIijA");

            var subscriptionRequest = new SubscribeRequest()
                .WithEndpoint(clientArn)
                .WithProtocol("sqs")
                .WithTopicArn("arn:aws:sns:us-east-1:451419498740:EventMessage");
            
            var response = sns.Subscribe(subscriptionRequest);
            Console.WriteLine(response.SubscribeResult.SubscriptionArn);
        }
        [Test]
        public void list_queues()
        {
            var response = _client.ListQueues(new ListQueuesRequest());
            foreach (var result in response.ListQueuesResult.QueueUrl)
            {
                Console.WriteLine(result);
            }
            var attribute = _client.GetQueueAttributes(
                new GetQueueAttributesRequest().WithQueueUrl(_queueUrl).WithAttributeName("QueueArn"));
            foreach (var att in attribute.GetQueueAttributesResult.Attribute)
            {
                Console.WriteLine(att.Value);
            }

        }
        [Test]
        public void set_permissions()
        {
            var setQueueAttributeRequest = new SetQueueAttributesRequest()
                .WithQueueUrl(_queueUrl)
                .WithAttribute(new Attribute { Name = "Policy", Value = AllowSnsAttribute() });
            var result = _client.SetQueueAttributes(setQueueAttributeRequest);
            Console.WriteLine(result.ToXML());

            var getQueueAttributesResponse = _client.GetQueueAttributes(
                new GetQueueAttributesRequest().WithAttributeName("Policy").WithQueueUrl(_queueUrl));
            foreach (var attribute in getQueueAttributesResponse.GetQueueAttributesResult.Attribute)
            {
                Console.WriteLine("{0} : {1}",attribute.Name,attribute.Value);
            }
        }
        private string AllowSnsAttribute()
        {
            return @"{""Version"": ""2008-10-17"",""Statement"": [{""Resource"": ""arn:aws:sqs:us-east-1:451419498740:Test_for_blog"", ""Effect"": ""Allow"", ""Sid"": ""1"", ""Action"": ""sqs:*"", ""Condition"": {""StringEquals"": {""aws:SourceArn"": ""arn:aws:sns:us-east-1:451419498740:EventMessage""}}, ""Principal"": {""AWS"": ""*""}}]}";
        }
        [Test]
        public void send_and_recieve_messages()
        {
            

            Console.WriteLine("Sending Message");
            var sendMessageRequest = new SendMessageRequest()
                .WithQueueUrl(_queueUrl)
                .WithMessageBody("Hello from the cloud");
            
            var sendResult = _client.SendMessage(sendMessageRequest);
            Console.WriteLine(sendResult.ToXML());

            Console.WriteLine("Receiving Message");
            var request =
                new ReceiveMessageRequest().
                WithQueueUrl(_queueUrl);

            var result = _client.ReceiveMessage(request);
            foreach (var message in result.ReceiveMessageResult.Message)
            {
                Console.WriteLine(message.Body);
                _client.DeleteMessage(
                    new DeleteMessageRequest().WithQueueUrl(_queueUrl).WithReceiptHandle(message.ReceiptHandle));
            }
        }
        [Test]
        public void receive_message()
        {
            var request =
              new ReceiveMessageRequest().
              WithQueueUrl(_queueUrl);
            var result = _client.ReceiveMessage(request);
            foreach (var message in result.ReceiveMessageResult.Message)
            {
                Console.WriteLine(message.Body);
            }
        }
        [Test]
        public void delete_all_messages_in_queue()
        {
            var request =
              new ReceiveMessageRequest().
              WithQueueUrl(_queueUrl);

            var result = _client.ReceiveMessage(request);
            foreach (var message in result.ReceiveMessageResult.Message)
            {
                Console.WriteLine("Deleting Message {0}",message.Body);
                var deleteResult = _client.DeleteMessage(new DeleteMessageRequest().WithQueueUrl(_queueUrl).WithReceiptHandle(message.ReceiptHandle));
                Console.WriteLine(deleteResult.ToXML());
            }
        }
        [Test]
        public void delete_queue()
        {
            var response = _client.DeleteQueue(
                new DeleteQueueRequest().WithQueueUrl("https://queue.amazonaws.com/451419498740/Ellemy_CQRS_Example"));
            Console.WriteLine(response.ToXML());
        }
    }
}