@Library('jenkins-pipeline-shared-library') _
pipeline { 
    agent { label "Build" }
    options {
        skipStagesAfterUnstable()      
        parallelsAlwaysFailFast()  // This will always override the other timeouts in the pipeline. To be redefined.
    }
    environment { 
        // Provider specific
        PROVIDER_NAME = "launch"
        PROVIDER_PROJECT_NAME = "Launch" // SOLUTION NAME
        PROVIDER_REPO_NAME = "gaming-integrations-game-launch-webapi" 

        // Credentials
        DEV_NEW_RELIC_APP_NAME = credentials('gi-game-launch-dev.NEW_RELIC_APP_NAME')
        DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB = credentials('gi-game-launch-dev.ConnectionStrings__GAMING_INTEGRATION_API_DB')
        DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB = credentials('gi-game-launch-dev.ConnectionStrings__GAMING_INTEGRATION_BUSINESS_ENGINE_DB')
        DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB = credentials('gi-game-launch-dev.ConnectionStrings__GAMING_INTEGRATION_USERS_DB')
        DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB = credentials('gi-game-launch-dev.ConnectionStrings__GAMING_INTEGRATION_WALLET_DB')
        DEV_CONNECTIONSTRINGS_REDIS = credentials('gi-redis-dev.ConnectionStrings__REDIS')
        DEV_NEW_RELIC_LICENSE_KEY = credentials('shared.dev.NEW_RELIC_LICENSE_KEY')

        QA_NEW_RELIC_APP_NAME = credentials('gi-game-launch-qa.NEW_RELIC_APP_NAME')
        QA_CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB = credentials('gi-game-launch-qa.ConnectionStrings__GAMING_INTEGRATION_API_DB')
        QA_CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB = credentials('gi-game-launch-qa.ConnectionStrings__GAMING_INTEGRATION_BUSINESS_ENGINE_DB')
        QA_CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB = credentials('gi-game-launch-qa.ConnectionStrings__GAMING_INTEGRATION_USERS_DB')
        QA_CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB = credentials('gi-game-launch-qa.ConnectionStrings__GAMING_INTEGRATION_WALLET_DB')
        QA_CONNECTIONSTRINGS_REDIS = credentials('gi-redis-qa.ConnectionStrings__REDIS')
        QA_NEW_RELIC_LICENSE_KEY = credentials('shared.qa.NEW_RELIC_LICENSE_KEY')

        PPD_NEW_RELIC_APP_NAME = credentials('gi-game-launch-ppd.NEW_RELIC_APP_NAME')
        PPD_CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB = credentials('gi-game-launch-ppd.ConnectionStrings__GAMING_INTEGRATION_API_DB')
        PPD_CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB = credentials('gi-game-launch-ppd.ConnectionStrings__GAMING_INTEGRATION_BUSINESS_ENGINE_DB')
        PPD_CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB = credentials('gi-game-launch-ppd.ConnectionStrings__GAMING_INTEGRATION_USERS_DB')
        PPD_CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB = credentials('gi-game-launch-ppd.ConnectionStrings__GAMING_INTEGRATION_WALLET_DB')
        PPD_CONNECTIONSTRINGS_REDIS = credentials('gi-redis-ppd.ConnectionStrings__REDIS')
        PPD_NEW_RELIC_LICENSE_KEY = credentials('shared.ppd.NEW_RELIC_LICENSE_KEY')

        PROD_NEW_RELIC_APP_NAME = credentials('gi-game-launch-prod.NEW_RELIC_APP_NAME')
        PROD_CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB = credentials('gi-game-launch-prod.ConnectionStrings__GAMING_INTEGRATION_API_DB')
        PROD_CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB = credentials('gi-game-launch-prod.ConnectionStrings__GAMING_INTEGRATION_BUSINESS_ENGINE_DB')
        PROD_CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB = credentials('gi-game-launch-prod.ConnectionStrings__GAMING_INTEGRATION_USERS_DB')
        PROD_CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB = credentials('gi-game-launch-prod.ConnectionStrings__GAMING_INTEGRATION_WALLET_DB')
        PROD_CONNECTIONSTRINGS_REDIS = credentials('gi-redis-prod.ConnectionStrings__REDIS')
        PROD_NEW_RELIC_LICENSE_KEY = credentials('shared.prod.NEW_RELIC_LICENSE_KEY')
    }
    stages {
        stage ("Build Docker Tag"){
            steps{ script{ 
                env.IMAGE_BRANCH_NAME = "${env.BRANCH_NAME}".take(40).replaceAll('/', '-')
                env.DOCKER_TAG = "${GIT_COMMIT}.${IMAGE_BRANCH_NAME}.${env.BUILD_ID}"
            }}
        }
        stage("Build URI Images"){
            steps{ script{
                env.IMAGE_REPO = "${NEXUS_REGISTRY}/gaming-integrations/providers/${PROVIDER_NAME}"
                env.IMAGE_URI = "${IMAGE_REPO}/api:${DOCKER_TAG}"
                env.IMAGE_URI_TESTS = "${IMAGE_REPO}/tests:${DOCKER_TAG}"
            }}
        }
        stage('Build Docker Images') {
            agent { label 'Build' }
            environment {
                CI = true
            }
            stages {
                stage ("Set up base image") {
                    environment { IMAGE_URI_BUILD = "${IMAGE_REPO}/build:${DOCKER_TAG}" }
                    steps {
                        sh "docker pull ${IMAGE_URI_BUILD} || true"
                        sh "docker build --no-cache --target build --network host -t ${IMAGE_URI_BUILD} ."
                }}
                stage("Build Images") {
                    parallel {
                        stage('Release Image') {
                            steps {
                                sh "docker pull ${IMAGE_URI} || true"
                                sh "docker build --target release --network host -t ${IMAGE_URI} ."
                                sh "docker push ${IMAGE_URI}"
                        }}
                        stage ('Tests Image') {
                            steps {
                                sh "docker pull ${IMAGE_URI_TESTS} || true"
                                sh "docker build --target test-release --network host -t ${IMAGE_URI_TESTS} ."
                                sh "docker push ${IMAGE_URI_TESTS}"
        }}}}}}
        stage('Testing') {
            agent { label 'Test' }
            environment {
                CI = true
            }
            stages {
                stage('Run') {
                    parallel {
                        stage('Unit Testing') { steps { sh "docker run --rm -i ${IMAGE_URI_TESTS} dotnet test tests/${PROVIDER_PROJECT_NAME}.Unit.Tests.Api.dll" }}
                        stage('Mocked Integration Testing') { steps { sh "docker run --rm -i ${IMAGE_URI_TESTS} dotnet test tests/${PROVIDER_PROJECT_NAME}.Integration.Tests.Api.dll"
        }}}}}}
        stage('Deploy for Feature branches'){
            when { not { anyOf {
                                branch 'development'
                                branch 'release'
                                branch 'main'
                                branch 'hotfix/*'                        
            }}}
            stages{
                stage('DEV'){
                    agent {  label 'Mainline' }
                    environment {
                        ENV_NAME = "dev"
                        CI = true
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB = "${DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB = "${DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB = "${DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB = "${DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}"
                        CONNECTIONSTRINGS_REDIS = "${DEV_CONNECTIONSTRINGS_REDIS}"
                        NEW_RELIC_LICENSE_KEY = "${DEV_NEW_RELIC_LICENSE_KEY}"
                        NEW_RELIC_APP_NAME = "${DEV_NEW_RELIC_APP_NAME}"
                    }
                    stages {
                        stage('Approve') {
                            steps { timeout(time: 30, unit: 'MINUTES') { script{ input(message: 'Promote feature branch to DEV?', ok: 'Proceed') }}}}
                        stage('Promotion to DEV approved') {
                            stages {
                                stage('Deploy to DEV') {
                                    steps {
                                        // TODO: Tag docker image as deployed in Docker Registry with ENV_NAME (+ "Instance" - if we have multiple instances) tag
                                        dir('deploy') {
                                            ansiblePlaybook(
                                                playbook: "playbook.yml",
                                                inventory: "inventory/${ENV_NAME}",
                                                credentialsId: 'svc.jenkins',                                              
                                                extras: '-e CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}" -e CONNECTIONSTRINGS_REDIS="${CONNECTIONSTRINGS_REDIS}" -e NEW_RELIC_LICENSE_KEY=${NEW_RELIC_LICENSE_KEY} -e NEW_RELIC_APP_NAME=${NEW_RELIC_APP_NAME}')
                                }}}
                                stage('Record newrelic deployment') {
                                    steps {
                                        recordNewrelicDeploy environment: "${ENV_NAME}",
                                            appDeploy: "gi-${PROVIDER_NAME}-${ENV_NAME}",
                                            changelog: "https://bitbucket.itspty.com/projects/PI/repos/${PROVIDER_REPO_NAME}/browse/CHANGELOG.md?at=refs%2Fheads%2F${BRANCH_NAME}",
                                            description: "deployed ${GIT_COMMIT} (branch ${BRANCH_NAME})",
                                            version: "${GIT_COMMIT}",
                                            user: 'jenkins'
        }}}}}}}}
        stage('Deploy for Development branch'){
            when { branch 'development' }
            stages{
                stage('DEV'){
                    agent {  label 'Mainline' }
                    environment {
                        ENV_NAME = "dev"
                        CI = true
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB = "${DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB = "${DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB = "${DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB = "${DEV_CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}"
                        CONNECTIONSTRINGS_REDIS = "${DEV_CONNECTIONSTRINGS_REDIS}"
                        NEW_RELIC_LICENSE_KEY = "${DEV_NEW_RELIC_LICENSE_KEY}"
                        NEW_RELIC_APP_NAME = "${DEV_NEW_RELIC_APP_NAME}"          
                    }
                    stages {
                        stage('Approve') {
                            steps { timeout(time: 30, unit: 'MINUTES') { script { input(message: 'Promote feature branch to DEV?', ok: 'Proceed') }}}}
                        stage('Promotion to DEV approved') {
                            stages {                              
                                stage('Deploy to DEV') {
                                    steps {
                                        // TODO: Tag docker image as deployed in Docker Registry with ENV_NAME (+ "Instance" - if we have multiple instances) tag
                                        dir('deploy') {
                                            ansiblePlaybook(
                                                playbook: "playbook.yml",
                                                inventory: "inventory/${ENV_NAME}",
                                                credentialsId: 'svc.jenkins',
                                                extras: '-e CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}" -e CONNECTIONSTRINGS_REDIS="${CONNECTIONSTRINGS_REDIS}" -e NEW_RELIC_LICENSE_KEY=${NEW_RELIC_LICENSE_KEY} -e NEW_RELIC_APP_NAME=${NEW_RELIC_APP_NAME}')
                                }}}
                                stage('Record newrelic deployment') {
                                    steps {
                                        recordNewrelicDeploy environment: "${ENV_NAME}",
                                            appDeploy: "gi-${PROVIDER_NAME}-${ENV_NAME}",
                                            changelog: "https://bitbucket.itspty.com/projects/PI/repos/${PROVIDER_REPO_NAME}/browse/CHANGELOG.md?at=refs%2Fheads%2F${BRANCH_NAME}",
                                            description: "deployed ${GIT_COMMIT} (branch ${BRANCH_NAME})",
                                            version: "${GIT_COMMIT}",
                                            user: 'jenkins'
        		}}}}}}
            
 
                stage ('QA') {
                    agent { label "Mainline" }
                    environment {
                        ENV_NAME = "qa"
                        CI = true
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB = "${QA_CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB = "${QA_CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB = "${QA_CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB = "${QA_CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}"
                        CONNECTIONSTRINGS_REDIS = "${QA_CONNECTIONSTRINGS_REDIS}"
                        NEW_RELIC_LICENSE_KEY = "${QA_NEW_RELIC_LICENSE_KEY}"
                        NEW_RELIC_APP_NAME = "${QA_NEW_RELIC_APP_NAME}"
                    }
                    stages {
                        stage('Approve') {
                            steps { timeout(time: 1, unit: 'DAYS') { script { input(message: 'Promote to QA?', ok: 'Proceed') }}}}
                        stage('Promotion to QA approved') {
                            stages {
                                stage('Deploy to QA'){
                                    steps {
                                        // TODO: Tag docker image as deployed in Docker Registry with ENV_NAME (+ "Instance" - if we have multiple instances) tag
                                        dir('deploy') {
                                            ansiblePlaybook(
                                                playbook: "playbook.yml",
                                                inventory: "inventory/${ENV_NAME}",
                                                credentialsId: 'svc.jenkins',
                                                extras: '-e CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}" -e CONNECTIONSTRINGS_REDIS="${CONNECTIONSTRINGS_REDIS}" -e NEW_RELIC_LICENSE_KEY=${NEW_RELIC_LICENSE_KEY} -e NEW_RELIC_APP_NAME=${NEW_RELIC_APP_NAME}')
                                }}}
                                stage('Record newrelic deployment') {
                                    steps {
                                        recordNewrelicDeploy environment: "${ENV_NAME}",
                                            appDeploy: "gi-${PROVIDER_NAME}-${ENV_NAME}",
                                            changelog: "https://bitbucket.itspty.com/projects/PI/repos/${PROVIDER_REPO_NAME}/browse/CHANGELOG.md?at=refs%2Fheads%2F${BRANCH_NAME}",
                                            description: "deployed ${GIT_COMMIT} (branch ${BRANCH_NAME})",
                                            version: "${GIT_COMMIT}",
                                            user: 'jenkins'
                }}}}}}              
              
            }}
        stage('Deploy for Release branch'){
            when { anyOf {
                	branch 'release'
                	branch 'hotfix/*'
            }}
            stages{
               
                stage ('PPD') {
                    agent { label "Mainline" }
                    environment {
                        ENV_NAME = "ppd"
                        CI = true
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB = "${PPD_CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB = "${PPD_CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB = "${PPD_CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB = "${PPD_CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}"
                        CONNECTIONSTRINGS_REDIS = "${PPD_CONNECTIONSTRINGS_REDIS}"
                        NEW_RELIC_LICENSE_KEY = "${PPD_NEW_RELIC_LICENSE_KEY}"
                        NEW_RELIC_APP_NAME = "${PPD_NEW_RELIC_APP_NAME}"
                    }
                    stages {
                        stage('Approve') {
                            steps { timeout(time: 1, unit: 'DAYS') { script { input(message: 'Promote to PPD?', ok: 'Proceed') }}}}
                        stage('Promotion to PPD approved') {
                            stages{
                                stage('Deploy to PPD') {
                                    steps {
                                        // TODO: Tag docker image as deployed in Docker Registry with ENV_NAME (+ "Instance" - if we have multiple instances) tag
                                        dir('deploy') {
                                            ansiblePlaybook(
                                                playbook: "playbook.yml",
                                                inventory: "inventory/${ENV_NAME}",
                                                credentialsId: 'svc.jenkins',
                                                extras: '-e CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}" -e CONNECTIONSTRINGS_REDIS="${CONNECTIONSTRINGS_REDIS}" -e NEW_RELIC_LICENSE_KEY=${NEW_RELIC_LICENSE_KEY} -e NEW_RELIC_APP_NAME=${NEW_RELIC_APP_NAME}')
                                }}}
                                stage('Record newrelic deployment') {
                                    steps {
                                        recordNewrelicDeploy environment: "${ENV_NAME}",
                                            appDeploy: "gi-${PROVIDER_NAME}-${ENV_NAME}",
                                            changelog: "https://bitbucket.itspty.com/projects/PI/repos/${PROVIDER_REPO_NAME}/browse/CHANGELOG.md?at=refs%2Fheads%2F${BRANCH_NAME}",
                                            description: "deployed ${GIT_COMMIT} (branch ${BRANCH_NAME})",
                                            version: "${GIT_COMMIT}",
                                            user: 'jenkins'
        }}}}}}}}
        stage('Deploy for Main branch'){
            when { branch 'main' }
            stages{
                
                stage ('PROD'){
                    agent { label 'Mainline' }
                    environment {
                        CI = true
                        ENV_NAME = "prod"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB = "${PROD_CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB = "${PROD_CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB = "${PROD_CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}"
                        CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB = "${PROD_CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}"
                        CONNECTIONSTRINGS_REDIS = "${PROD_CONNECTIONSTRINGS_REDIS}"
                        NEW_RELIC_LICENSE_KEY = "${PROD_NEW_RELIC_LICENSE_KEY}"
                        NEW_RELIC_APP_NAME = "${PROD_NEW_RELIC_APP_NAME}"
                    }
                    stages {
                        stage('Approve') { 
                            steps { timeout(time: 1, unit: 'DAYS') { script{ input(message: 'Promote to PROD?', ok: 'Proceed') }}}}
                        stage('Promotion to PROD approved') {
                            stages {
                                stage('Deploy to PROD') {
                                    steps {
                                        // TODO: Tag docker image as deployed in Docker Registry with ENV_NAME (+ "Instance" - if we have multiple instances) tag
                                        dir('deploy') {
                                            ansiblePlaybook(
                                                playbook: "playbook.yml",
                                                inventory: "inventory/${ENV_NAME}",
                                                credentialsId: 'svc.jenkins',
                                                extras: '-e CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}" -e CONNECTIONSTRINGS_REDIS="${CONNECTIONSTRINGS_REDIS}" -e NEW_RELIC_LICENSE_KEY=${NEW_RELIC_LICENSE_KEY} -e NEW_RELIC_APP_NAME=${NEW_RELIC_APP_NAME}')
                                }}}
                                stage('Record newrelic deployment') {
                                    steps {
                                        recordNewrelicDeploy environment: "${ENV_NAME}",
                                            appDeploy: "gi-${PROVIDER_NAME}-${ENV_NAME}",
                                            changelog: "https://bitbucket.itspty.com/projects/PI/repos/${PROVIDER_REPO_NAME}/browse/CHANGELOG.md?at=refs%2Fheads%2F${BRANCH_NAME}",
                                            description: "deployed ${GIT_COMMIT} (branch ${BRANCH_NAME})",
                                            version: "${GIT_COMMIT}",
                                            user: 'jenkins'
                                }}  
                                stage('Rollback') {
                                    stages {
                                        stage('Approve rollback?') {
                                            steps { timeout(time: 2, unit: 'DAYS') { script{ input(message: 'Rollback to previous deploy?', ok: 'Proceed') }}}}
                                        stage('Rollback approved') {
                                            stages {
                                                stage('Rollback to previous deploy') {
                                                    agent { label "Mainline" }
                                                    steps {
                                                        dir('deploy') {
                                                            ansiblePlaybook(
                                                                playbook: "rollback.yml",
                                                                inventory: "inventory/${ENV_NAME}",
                                                                credentialsId: 'svc.jenkins',
                                                                extras: '-e CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_API_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_BUSINESS_ENGINE_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_USERS_DB}" -e CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB="${CONNECTIONSTRINGS_GAMING_INTEGRATION_WALLET_DB}" -e CONNECTIONSTRINGS_REDIS="${CONNECTIONSTRINGS_REDIS}" -e NEW_RELIC_LICENSE_KEY=${NEW_RELIC_LICENSE_KEY} -e NEW_RELIC_APP_NAME=${NEW_RELIC_APP_NAME}')
}}}}}}}}}}}}}}}
