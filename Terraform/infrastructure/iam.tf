module "ecs_iam" {
  source                 = "git@ghe.coxautoinc.com:coxauto-europe/cmr-terraform-modules.git//conf/iam-ecs?ref=v0.0.2-tf-m4"
  environment            = var.environment
  service_name           = "payment"
  vault_dlq_access       = false
  eventbridge_dlq_access = false
  ecs_task_role_policy   = data.aws_iam_policy_document.ecs_task_role_policy.json
}

data "aws_iam_policy_document" "ecs_task_role_policy" {
  source_policy_documents = compact([
    data.aws_iam_policy_document.secrets_policy.json,
    data.aws_iam_policy_document.ssm_policy.json,
    var.audit_active == true ? data.aws_iam_policy_document.dynamo_audit_policy[0].json : "",
    data.aws_iam_policy_document.dynamo_policy.json
  ])
}

data "aws_iam_policy_document" "secrets_policy" {
  statement {
    effect = "Allow"
    actions = [
      "secretsmanager:GetSecretValue"
    ]
    resources = ["arn:aws:secretsmanager:${var.region}:${data.aws_caller_identity.current.account_id}:secret:${var.project}-${var.environment}-service-payment*",
      "arn:aws:secretsmanager:${var.region}:${data.aws_caller_identity.current.account_id}:secret:${var.project}-${var.environment}-global*",
      "arn:aws:secretsmanager:${var.region}:${data.aws_caller_identity.current.account_id}:secret:${var.project}-${var.environment}-api-keys*"
    ]
  }
}

data "aws_iam_policy_document" "ssm_policy" {
  #checkov:skip=CKV_AWS_108
  #checkov:skip=CKV_AWS_111
  statement {
    effect = "Allow"
    actions = [
      "ssm:PutParameter",
      "ssm:GetParametersByPath"
    ]
    resources = ["*"]
  }
}

data "aws_iam_policy_document" "dynamo_policy" {
  statement {
    effect = "Allow"
    actions = [
      "dynamodb:List*",
      "dynamodb:DescribeReservedCapacity*",
      "dynamodb:DescribeLimits",
      "dynamodb:DescribeTimeToLive"
    ]
    resources = ["*"]
  }
  statement {
    effect = "Allow"
    actions = [
      "dynamodb:BatchGet*",
      "dynamodb:DescribeStream",
      "dynamodb:DescribeTable",
      "dynamodb:Get*",
      "dynamodb:Query",
      "dynamodb:Scan",
      "dynamodb:BatchWrite*",
      "dynamodb:CreateTable",
      "dynamodb:Delete*",
      "dynamodb:Update*",
      "dynamodb:PutItem"
    ]
    resources = [aws_dynamodb_table.payment_method.arn, "${aws_dynamodb_table.payment_method.arn}/*"]
  }
}

data "aws_iam_policy_document" "dynamo_audit_policy" {
  count = var.audit_active == true ? 1 : 0
  statement {
    effect = "Allow"
    actions = [
      "dynamodb:BatchGet*",
      "dynamodb:DescribeTable",
      "dynamodb:Get*",
      "dynamodb:Query",
      "dynamodb:Scan"
    ]
    resources = [data.aws_dynamodb_table.audit_table[0].arn, "${data.aws_dynamodb_table.audit_table[0].arn}/*"]
  }
}

data "aws_dynamodb_table" "audit_table" {
  count = var.audit_active == true ? 1 : 0
  name  = "${var.project}-${var.environment}-payment-method-audit"
}
