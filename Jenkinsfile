pipeline {

    agent any
    environment {
        NEW_VERSION = '1.0.0'
    }

    stages {

        stage("build") {
            when {
                expression {
                    BRANCH_NAME == 'dev' && CODE_CHANGES == true
                }
            }
            steps {
               echo "building the application..."
               echo "building version ${NEW_VERSION}"
               echo 'building version ${NEW_VERSION}'
            }
        }

        stage("test") {
            when {
                expression {
                    BRANCH_NAME == 'dev' || BRANCH_NAME == 'master'
                }
            }
            steps {
               echo "testing the application..."
            }
        }

        stage("deploy") {

            steps {
               echo "deploying the application..."
            }
        }
    }
}