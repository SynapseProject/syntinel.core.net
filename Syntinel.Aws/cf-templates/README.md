# Syntinel
## Cloud Formation Templates

The cloud formation template "cft-syntinel.yaml" creates all the necessary resources required to run Syntinel in your AWS account.  

This template calls the cloud formation templates in the "stacks" directory, passing the required information into each template.  It assumes you are running the template with a role that has the permissions to create the resources necessary (listed below).  

**Required Permissions**
* Create IAM Roles and Policies
* Create Lambda Functions
* Create DynamoDb Tables
* Create APIGateway Resources
* Deploy APIGateway Resources

## Running the Templates Separately

If the permissions you need to deploy Syntinel are spread across multiple roles, run each template in the "stacks" directory individually in the following order : 

* cft-syntinel-init
* cft-syntinel-iam
* cft-syntinel-database
* cft-syntinel-lambda
* cft-syntinel-apigateway
* cft-syntinel-autoupdater (optional)

## Template Variables

Most of the variables in the templates allow for customization when certain naming standards are required to be maintained.  The majority of the variables can be left as their default value with the following excpetions : 

* **S3BucketName :** The bucket name where these templates and the lambda source code zip file reside.  Must be in the same region as you intend to deploy Syntinel.
* **S3BucketPrefix :** If you have the Syntinel deployment artifacts in a sub-directory of a bucket, add that directory here.
* **PolicyPermissionBoundry :** If you are required to add a permission boundry on your IAM roles, include that value here.
* **xxxxxStackName :** If running each template separately, some of the templates require the stack name of the previously run templates.  
