<script>
import { okCancle } from '@/components/iPrompt'
import SelectEmail from '../components/selectEmail'

export default {
  data() {
    return {
      // 发件箱
      senders: []
    }
  },

  methods: {
    // 打开发件人选择框
    async openSelectSendersDialog() {
      // 打开
      const res = await okCancle('选择发件人', '', {
        component: SelectEmail,
        parent: this, // 成为该Vue节点的子元素
        value: this.senders,
        groupType: 1
      })

      console.log('openSelectSendersDialog:', res)

      if (!res) return

      this.senders = res.data
    },

    // 移除发件人
    removeSender(sender) {
      const index = this.senders.findIndex(
        re => re.type === sender.type && re._id === sender._id
      )
      if (index > -1) this.senders.splice(index, 1)
    }
  }
}
</script>
