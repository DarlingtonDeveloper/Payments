region                  = "eu-west-1"
project                 = "cmr"
environment             = "test"
aws_account_name        = "awseucommerce2np"
network_environment_tag = "np"
workload                = "cmr"
audit_active            = false

dynamodb_tags = {
  "coxauto:ci-id" = "CI2402996"
}
