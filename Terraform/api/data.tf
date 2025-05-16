data "aws_subnet" "cmr_subnet_1a_private" {
  tags = {
    Name        = "${var.aws_account_name}-private-${var.region}a"
    SUB-Type    = "Private"
    Environment = var.network_environment_tag
  }
}

data "aws_subnet" "cmr_subnet_1b_private" {
  tags = {
    Name        = "${var.aws_account_name}-private-${var.region}b"
    SUB-Type    = "Private"
    Environment = var.network_environment_tag
  }
}

data "aws_iam_role" "ecs_task_role" {
  name = "${var.project}-${var.environment}-payment-ecs-task-role"
}

data "aws_caller_identity" "current" {}
