pipeline {
    agent any

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
                withCredentials([string(credentialsId: 'NotionAPIToken', variable: 'NOTION_TOKEN')]) {
                    // JSON 파일 생성
                    writeFile file: 'notion.json', text: """{
                        "parent": { "database_id": "26ccf41cca108003b25bd851f63aac87" },
                        "properties": {
                            "BuildName": {
                                "title": [
                                    { "text": { "content": "AITest Build #${BUILD_NUMBER}" } }
                                ]
                            },
                            "상태": {
                                "rich_text": [
                                    { "text": { "content": "SUCCESS: Jenkins build" } }
                                ]
                            }
                        }
                    }"""

                    // Notion API 호출
                    bat """
                        curl -X POST "https://api.notion.com/v1/pages" ^
                        -H "Authorization: Bearer %NOTION_TOKEN%" ^
                        -H "Content-Type: application/json" ^
                        -H "Notion-Version: 2022-06-28" ^
                        -d @notion.json
                    """
                }
            }
        }
    }
}
