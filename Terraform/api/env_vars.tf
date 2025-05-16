locals {
  env_vars = [
    {
      name  = "ASPNETCORE_ENVIRONMENT"
      value = var.environment
    },
    {
      name  = "NEW_RELIC_APP_NAME"
      value = "${var.project}-${var.environment}-payment-api"
    },
    {
      name  = "Serilog__WriteTo__0__Args__configure__0__Args__applicationName"
      value = "${var.project}-${var.environment}-payment-api"
    }
  ]
  secrets = [
    # example adding a new secret to app env vars, update values in <>
    # {
    #   name = "<MY_SECRET>"
    #   valueFrom = data.aws_secretsmanager_secret.<my_secret>.arn
    # }
    {
      name      = "NEW_RELIC_LICENSE_KEY"
      valueFrom = "${data.aws_secretsmanager_secret.global.arn}:new-relic-license-key::"
    },
    {
      name      = "Serilog__WriteTo__0__Args__configure__0__Args__licenseKey"
      valueFrom = "${data.aws_secretsmanager_secret.global.arn}:new-relic-license-key::"
    },
    {
      name      = "ApiKeys"
      valueFrom = data.aws_secretsmanager_secret.api_keys.arn
    },
    {
      name      = "Application__ApiKey"
      valueFrom = "${data.aws_secretsmanager_secret.payment.arn}:api-key::"
    }
  ]
}

data "aws_secretsmanager_secret" "payment" {
  name = "${var.project}-${var.environment}-service-payment"
}

data "aws_secretsmanager_secret" "api_keys" {
  name = "${var.project}-${var.environment}-api-keys"
}

data "aws_secretsmanager_secret" "global" {
  name = "${var.project}-${var.environment}-global"
}
