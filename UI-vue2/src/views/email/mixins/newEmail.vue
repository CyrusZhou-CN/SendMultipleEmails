<script>
import { newEmail } from '@/api/group'
import { notifySuccess } from '@/components/iPrompt'

const emailCommonInfo = [
  {
    name: 'groupId',
    type: 'text',
    label: '组id',
    required: true,
    readonly: false,
    hidden: true
  },
  {
    name: 'email',
    type: 'email',
    label: '邮箱',
    required: true
  },
  {
    name: 'description',
    type: 'text',
    label: '描述',
    required: true
  }
]

const emailSender = [
  {
    name: 'smtpAddress',
    type: 'text',
    label: 'smtp服务器',
    required: true
  },
  {
    name: 'smtpPassword',
    type: 'password',
    label: 'smtp密码',
    required: true
  },
  {
    name: 'smtpProtocol',
    type: 'text',
    label: 'smtp协议',
    required: true,
    default: 'https'
  },
  {
    name: 'smtpPort',
    type: 'number',
    label: 'smtp端口',
    required: true,
    default: 465
  }
]

export default {
  data() {
    return {
      isShowNewEmailDialog: false,
      initNewEmailParams: {
        title: '新增',
        tooltip: '',
        api: newEmail,
        // type 可接受的值：text/password/textarea/email/search/tel/file/number/url/time/date
        fields: []
      }
    }
  },
  computed: {
    newEmailTitle() {
      if (this.group.groupType === 1) {
        return '新增发件箱'
      }
      return '新增收件箱'
    }
  },
  methods: {
    openNewEmailDialog() {
      // 添加 fields
      const fields = [...emailCommonInfo]
      if (this.group.groupType === 1) {
        fields.push(...emailSender)
      }
      fields[0].default = this.group._id
      this.initNewEmailParams.fields = fields
      this.initNewEmailParams.title = this.newEmailTitle

      this.isShowNewEmailDialog = true
    },

    addedNewEmail(data) {
      // 查看是否存在，如果存在，替换
      const index = this.data.findIndex(d => d._id === data._id)
      if (index > -1) this.data.splice(index, 1, data)
      else this.data.push(data)

      this.isShowNewEmailDialog = false
      notifySuccess('添加成功')
    }
  }
}
</script>
