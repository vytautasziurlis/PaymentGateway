# PaymentGatewayAPI

PaymentGateway is a REST API that processes payment requests from merchants and provides a way to get payment details by reference.

# Prerequisites

* .NET 5.0
* IDE supporting C# 9.0

# Getting started

PaymentGatewayAPI can be started locally or as a Docker container. After cloning repository it can be started locally by building solution and starting PaymentGateway.API project.

To run as a Docker container please follow steps below:
* Build the container by running the following command:

`docker build -f Dockerfile -t "payment-gateway-api" .`

* Start the container using the following command:

`docker run -dp 5000:80 payment-gateway-api`

SwaggerUI is exposed on `/swagger`. Postman collection can be downloaded from [here](PaymentGatewayAPI.postman_collection.json).

PaymentGatewayAPI also exposes Prometheus metrics on port 8080.

# Bank API simulator

At the moment PaymentGatewayAPI uses mock bank service. It simulates responses from the bank as follows:
* If card number starts with "5" (i.e. MasterCard) - return `Failure`
* In all other cases return `Success`

The following valid card number can be used for testing purposes: `4689387567825`.

# TODO

* Add authentication support. Once authentication is implemented - update domain logic to ensure that merchants can only get details of the payment they created
* Add support for data encryption and replace mock repository
* Replace manual mapper with more efficient dynamic mapper (e.g. Mapster)
* Add validation into MediatR pipeline
* Improve card number masking logic
* Improve exception handling at the API level
* Refactor CardNumber and CardCvv classes to make them more efficient
* Review Luhn check NuGet package and implement more efficient algorithm if required
* Add CancellationToken support
* Implement API client
