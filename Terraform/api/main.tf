module "ecs_payment" {
  source                 = "git@ghe.coxautoinc.com:coxauto-europe/cmr-terraform-modules.git//conf/ecs-app?ref=v0.0.1-tf-m3"
  ecs_service_name       = "payment"
  ecr_version            = var.ecr_version
  image_name             = "cmr-payment"
  container_port         = 80
  ecs_service_subnet_ids = [data.aws_subnet.cmr_subnet_1a_private.id, data.aws_subnet.cmr_subnet_1b_private.id]
  env_vars               = local.env_vars
  secrets                = local.secrets
  ecs_task_role_arn      = data.aws_iam_role.ecs_task_role.arn

  project                 = var.project
  environment             = var.environment
  aws_account_name        = var.aws_account_name
  network_environment_tag = var.network_environment_tag

  ecr_repository_includes_environment = false
}
