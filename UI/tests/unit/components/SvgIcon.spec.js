import { shallowMount } from '@vue/test-utils';
import SvgIcon from '@/components/SvgIcon/index.vue';

describe('SvgIcon.vue', () => {
  it('iconClass', () => {
    const wrapper = shallowMount(SvgIcon, {
      props: {
        iconClass: 'test'
      }
    });
    expect(wrapper.find('use').attributes('xlink:href')).toBe('#icon-test');
  });

  it('className', async () => {
    const wrapper = shallowMount(SvgIcon, {
      props: {
        iconClass: 'test'
      }
    });
    expect(wrapper.classes()).toHaveLength(1);

    await wrapper.setProps({ className: 'test' }); // 使用await等待异步更新
    expect(wrapper.classes()).toContain('test');
  });
});
