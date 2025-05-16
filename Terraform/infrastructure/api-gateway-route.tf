module "api_deployment" {
  source      = "git@ghe.coxautoinc.com:coxauto-europe/cmr-terraform-modules.git//conf/api-gateway-deployment?ref=v0.0.1-tf-m2"
  environment = var.environment
  sha_key     = sha1(jsonencode(file("api-gateway-route.tf")))
  workload    = var.workload
}

module "payment_health" {
  source             = "git@ghe.coxautoinc.com:coxauto-europe/cmr-terraform-modules.git//conf/api-gateway-route?ref=v0.0.1-tf-m1"
  environment        = var.environment
  authorization_type = "NONE"
  http_route         = "/payment/health"
  http_method        = "GET"
  workload           = var.workload
}

module "payment_method_id_v1_get" {
  source             = "git@ghe.coxautoinc.com:coxauto-europe/cmr-terraform-modules.git//conf/api-gateway-route?ref=v0.0.1-tf-m1"
  environment        = var.environment
  authorization_type = "NONE"
  http_route         = "/payment/v1/payment-method/{paymentMethodId}"
  http_method        = "GET"
  workload           = var.workload
}

module "payment_method_v1_post" {
  source             = "git@ghe.coxautoinc.com:coxauto-europe/cmr-terraform-modules.git//conf/api-gateway-route?ref=v0.0.1-tf-m1"
  environment        = var.environment
  authorization_type = "NONE"
  http_route         = "/payment/v1/payment-method"
  http_method        = "POST"
  workload           = var.workload
}
