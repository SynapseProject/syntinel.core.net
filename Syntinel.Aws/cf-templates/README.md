# Syntinel
## Cloud Formation Templates

The cloud formation template "cft-syntinel.yaml" creates all the necessary resources required to run Syntinel in your AWS account.  

This template calls the cloud formation templates in the "stacks" directory, passing the required information into each template.  It assumes you are running the template with a policy that has the permissions to create the resources necessary (listed below).  The policy document can be found in the "policies" directory.

**Minimum Required Permissions**
- apigateway:
    - DELETE
    - GET
    - PATCH
    - POST
    - PUT
- cloudformation:
    - CreateStack
    - CreateStackInstances
    - DeleteStack
    - DeleteStackInstances
    - DescribeChangeSet
    - DescribeStackEvents
    - DescribeStacks
    - GetTemplate
    - GetTemplateSummary
    - ListStackInstances
    - ListStackResources
    - ListStacks
    - ListStackSetOperations
    - UpdateStack
    - UpdateStackInstances
    - ValidateTemplate
- dynamodb:
    - CreateTable
    - DeleteTable
    - DescribeTable
    - UpdateTable
- iam:
    - AttachRolePolicy
    - CreatePolicy
    - CreatePolicyVersion
    - CreateRole
    - DeletePolicy
    - DeletePolicyVersion
    - DeleteRole
    - DetachRolePolicy
    - GetPolicy
    - GetRole
    - ListAttachedRolePolicies
    - ListPolicies
    - ListPolicyVersions
    - ListRoles
    - PassRole
    - UpdateRole
- lambda:
    - AddPermission
    - CreateFunction
    - DeleteFunction
    - GetAccountSettings
    - GetFunction
    - GetFunctionConfiguration
    - ListFunctions
    - RemovePermission
    - UpdateFunctionCode
    - UpdateFunctionConfiguration
- s3:
    - GetObject

## Running the Templates Separately

If the permissions you need to deploy Syntinel are spread across multiple roles, run each template in the "stacks" directory individually in the following order : 

- cft-syntinel-init
- cft-syntinel-iam
- cft-syntinel-database
- cft-syntinel-lambda
- cft-syntinel-apigateway
- cft-syntinel-autoupdater (optional)

## Template Variables

Most of the variables in the templates allow for customization when certain naming standards are required to be maintained.  The majority of the variables can be left as their default value with the following excpetions : 

* **S3BucketName :** The bucket name where these templates and the lambda source code zip file reside.  Must be in the same region as you intend to deploy Syntinel.
* **S3BucketPrefix :** If you have the Syntinel deployment artifacts in a sub-directory of a bucket, add that directory here.
* **PolicyPermissionBoundry :** If you are required to add a permission boundry on your IAM roles, include that value here.
* **xxxxxStackName :** If running each template separately, some of the templates require the stack name of the previously run templates.  
