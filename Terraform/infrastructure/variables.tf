variable "region" {
  type = string
}

variable "project" {
  description = "Project name"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "workload" {
  type = string
}

variable "audit_active" {
  type = bool
}

variable "aws_account_name" {
  type = string
}

variable "network_environment_tag" {
  type = string
}

variable "tags_ecs" {
  description = "A map of tags to add to ECS resources"
  type        = map(string)
  default = {
    "coxauto:ci-id" = "CI2402991"
  }
}

variable "dynamodb_tags" {
  description = "A map of tags to add to organisation dynamodb resources"
  type        = map(string)
  default = {
    "coxauto:ci-id" = "CI2402996"
  }
}
