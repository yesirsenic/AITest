pipeline {
    agent any

    environment {
        // Jenkins Credentials에 등록한 Notion Secret Token ID
        NOTION_TOKEN = credentials('NotionAPIToken')
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main',
                    url: 'https://github.com/yesirsenic/AITest.git',
                    credentialsId: 'AITestGitHub'
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

        stage('Update Jira') {
            steps {
                jiraAddComment site: 'AITestJira', idOrKey: 'SCRUM-1', comment: '✅ Jenkins 빌드 성공!'
            }
        }

        stage('Update Notion') {
            steps {
                // Windows 환경에서는 sh 대신 bat 사용
                bat """
                curl -X POST "https://api.notion.com/v1/pages" ^
                    -H "Authorization: Bearer %NOTION_TOKEN%" ^
                    -H "Content-Type: application/json" ^
                    -H "Notion-Version: 2022-06-28" ^
                    -d "{ \\"parent\\":{\\"database_id\\":\\"26ccf41cca10809fbc2de77fc48aa2b5\\"}, \\"properties\\":{\\"빌드 이름\\":{\\"title\\":[{\\"text\\":{\\"content\\":\\"AITest Build #${BUILD_NUMBER}\\"}}]}, \\"Status\\":{\\"rich_text\\":[{\\"text\\":{\\"content\\":\\"✅ Jenkins 빌드 성공\\"}}]}}"
                """
            }
        }
    }
}
