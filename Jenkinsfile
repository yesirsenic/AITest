pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                git 'https://github.com/yesirsenic/AITest.git'
            }
        }
        stage('Build') {
            steps {
                echo 'Unity 프로젝트 빌드 단계 실행 중...'
            }
        }
        stage('Test') {
            steps {
                echo '테스트 실행 중...'
            }
        }
        stage('Deploy') {
            steps {
                echo '배포 단계 실행 중...'
            }
        }
    }
}