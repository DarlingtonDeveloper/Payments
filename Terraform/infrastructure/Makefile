env=test
service=payment
#export AWS_DEFAULT_PROFILE=non-prod-admin

echo:
	echo ${env}
	env | grep "TF"

## Initialize terraform remote state
init:
	terraform init \
		-backend-config="bucket=cmr-non-prod-l2-terraform-state-s3" \
		-backend-config="dynamodb_table=cmr-non-prod-l2-terraform-state-lock" \
		-backend-config="key=env:/test/service/${service}/infrastructure.tfstate" \
		-backend-config="region=eu-west-1" \
		-backend-config="encrypt=true"

pre-commit:
	terraform fmt
	terraform validate
	pre-commit run '--all-files'

## Clean up the project
clean:
	rm -rf .terraform *.tfstate*

## Pass arguments through to terraform which require remote state
console destroy graph output providers show: init
	terraform $@

## Plan need an environment file
plan : init
	terraform $@ -var-file=env/${env}.tfvars

## Pass arguments through to terraform which do not require remote state
get fmt validate version:
	terraform $@
