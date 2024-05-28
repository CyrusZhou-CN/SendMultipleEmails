<script>
import Path from 'path'
import ws from '@/utils/websocket'

export default {
  data() {
    return {
      attachments: []
    }
  },
  methods: {
    // 选择文件并发送到服务器
    async sendFilesToServer() {
      const input = document.createElement('input')
      input.type = 'file'
      input.multiple = true

      input.addEventListener('change', async event => {
        const files = Array.from(event.target.files)

        for (const file of files) {
          const reader = new FileReader()
          reader.onload = async e => {
            const fileContent = e.target.result

            // 通过 WebSocket 发送文件内容到服务器
            const result = await ws.sendRequest({
              name: 'SelectFiles',
              fileName: file.name,
              fileContent: fileContent.split(',')[1] // 移除Base64前缀
            })

            console.log('sendSignToSelectAttachment:', result)
            if (result.status !== 200) return
            this.attachments.push(result.result)
            console.log('add attachment:', this.attachments)
          }
          reader.readAsDataURL(file) // 读取文件内容为Base64编码
        }
      })

      // 触发文件选择对话框
      input.click()
    },

    // 触发选择
    async sendSignToSelectAttachment() {
      await this.sendFilesToServer()
    },
    // 获取fileName
    getFileBaseName(att) {
      var fileName = this.attachments.filter(
        f => f.fullName === att.fullName
      )[0].fileName
      return Path.basename(fileName)
    },
    async removeAttachment(att) {
      // 向服务器发送删除请求
      const result = await ws.sendRequest({
        name: 'DeleteFile',
        fileName: att.fullName
      })
      if (result.status !== 200) return
      // 移除选择的附件
      this.attachments = this.attachments.filter(
        f => f.fullName !== att.fullName
      )
    }
  }
}
</script>

<style>
/* 样式可以根据需要自行添加 */
</style>
