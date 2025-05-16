variable "region" {
  type = string
}

variable "ecr_version" {
  description = "ECR image version tag"
  type        = string
}

variable "project" {
  description = "Project name"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "aws_account_name" {
  type = string
}

variable "network_environment_tag" {
  type = string
}
