region                  = "eu-west-1"
project                 = "cmr"
environment             = "prod"
aws_account_name        = "awseucommerce2"
network_environment_tag = "prod"

audit_active = false

dynamodb_tags = {
  "coxauto:ci-id" = "CI2402996"
}
