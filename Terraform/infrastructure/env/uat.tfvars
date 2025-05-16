region                  = "eu-west-1"
project                 = "cmr"
environment             = "uat"
aws_account_name        = "awseucommerce2pp"
network_environment_tag = "pp"
workload                = "cmr"
audit_active            = false

dynamodb_tags = {
  "coxauto:ci-id" = "CI2402996"
}
