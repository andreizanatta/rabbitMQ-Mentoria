// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var nomeExchange = "exAndrei";

var person = new Person("Andrei", "document", new DateTime(1991, 09, 17));

var connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
};

var connection = connectionFactory.CreateConnection();

var channel = connection.CreateModel();

var json = JsonSerializer.Serialize(person);
var jsonBytes = Encoding.UTF8.GetBytes(json);

channel.BasicPublish(nomeExchange, "r.person", null, jsonBytes);

Console.WriteLine($"mensagem: {json}");

var consumerChannel = connection.CreateModel();

var consumer = new EventingBasicConsumer(consumerChannel);

consumer.Received += (sender, eventArgs) =>
{
    var jsonArray = eventArgs.Body.ToArray();
    var bytesArray = Encoding.UTF8.GetString(jsonArray);
    var objeto = JsonSerializer.Deserialize<Person>(bytesArray);

    Console.WriteLine($"mensagem recebida: {bytesArray}");

    consumerChannel.BasicAck(eventArgs.DeliveryTag, false);
};

consumerChannel.BasicConsume("queue", false, consumer);

Console.ReadLine();

public class Person
{
    public Person(string fullName, string document, DateTime birthDate)
    {
        FullName = fullName;
        Document = document;
        BirthDate = birthDate;
    }

    public string FullName { get; set; }
    public string Document { get; set; }
    public DateTime BirthDate { get; set; }
}