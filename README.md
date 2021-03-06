# Device Silhouette

##Disclaimer 

**Device Silhouette is a reference pattern for using
Azure IoT Hub in conjunction with Service Fabric. For typical needs of storing
device state information (metadata, configuration, conditions, etc.) we
recommend using the [device twin](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-device-twins), a first-class capability of Azure IoT Hub.**

##About

Device Silhouette is an open source framework developed by TED GE EMEA team in Microsoft.

The Device Silhouette is a solution for managing IoT device state in the cloud.
The "Silhouette" is a virtual device that replicates the physical device’s latest
state. Applications can interact with the Silhouette even when the device is
offline. A Silhouette persists the last reported state and desired future
state. You can retrieve the last reported state of a device or set a desired
future state through a Rest API.

The Silhouette holds information about all state messages sent and received
to/from the device, including delivery status updates. With the Silhouette
you can send desired state to the device, set a TTL for the request and check
if the message was delivered successfully or not. Every message sent increases
the version of the Silhouette state. Additionally you can retrieve the short
term history of all messages, this functionality enables building a rule engine,
state machine and solve conflicts.

## Documentation Index

2.	[Overview](Documentation/overview.md)
 * [Introduction - What is Device Silhouette?] (Documentation/overview.md#introduction---what-is-device-silhouette)
 * [Features](Documentation/overview.md#features)
 * [Benefits](Documentation/overview.md#benefits)
 * [Scenarios](Documentation/overview.md#scenarios)
3.	[How it works?](Documentation/howitworks.md)
 * [Silhouette Message](Documentation/silhouettemessage.md)
 * [Messaging endpoints and lifecycle](Documentation/howitworks.md#messaging-endpoints-and-lifecycle)
 * [Messages flow](Documentation/messagesflow.md)
 * [Messages purging and persistency](Documentation/howitworks.md#messages-purging-and-persistancy)
 * [REST API](Documentation/RESTAPI.md)
 * Example scenarios: [Home lightning](Documentation/lightsSampleScenario.md), [Oven maintenance](Documentation/ovenscenario.md)
4.	[Architecture](Documentation/architecture.md)
 * [Main components](Documentation/architecture.md#main-components)
 * [Service Fabric Services](Documentation/architecture.md#service-fabric-microservices)
5.	[Developer guide](Documentation/developerguide.md)
 * [Set the development environment](Documentation/devenvironment.md)
 * [Configuration](Documentation/configuration.md)
 * [Providers](Documentation/developerguide.md#providers)
 * [Test](Documentation/test.md)
6.	[Deploy to production](Documentation/deployment.md)
7.	[Security](Documentation/security.md)
 * [Devices security](Documentation/security.md#device-security)
 * [Service Fabric Security](Documentation/security.md#service-fabric-security)
 * [REST API Authentication and Authorization](Documentation/security.md#rest-api-authentication-and-authorization)



## Reporting issues and feedback

If you encounter any bugs with the tool please file an issue in the Issues
section of our GitHub repo.

## Contribute Code


We welcome contributions. To contribute please follow the instrctions on
[How to contribute?](CONTRIBUTING.md)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


## License

Device Silhouette is licensed under the MIT License.
