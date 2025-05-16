resource "aws_dynamodb_table" "payment_method" {
  #checkov:skip=CKV_AWS_119:Using AWS Managed KMS Key
  name                        = "${var.project}-${var.environment}-payment-method"
  hash_key                    = "PaymentMethodId"
  range_key                   = "SK"
  billing_mode                = "PAY_PER_REQUEST"
  deletion_protection_enabled = true
  stream_enabled              = true
  stream_view_type            = "NEW_AND_OLD_IMAGES"

  server_side_encryption {
    enabled = true
  }
  attribute {
    name = "PaymentMethodId"
    type = "S"
  }
  attribute {
    name = "SK"
    type = "S"
  }
  point_in_time_recovery {
    enabled = true
  }
  tags = var.dynamodb_tags
}
