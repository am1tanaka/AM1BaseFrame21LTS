# �V�[���̐؂�ւ�

���K�p�̐V�K�v���W�F�N�g������āA�N������V�[���̐؂�ւ����������Ă݂܂��傤�B
�ȉ��̊ȈՂȃQ�[�����[�v���������܂��B

- �N��(Boot)
  - ��ʂ��B�������ƃ{�����[���V�X�e���̏�����
  - �����I�Ƀ^�C�g���V�[���֐؂�ւ�
  - �؂�ւ���FilledRadial
- �^�C�g���V�[��(Title)
  - �N���b�N�����牼�̃Q�[���V�[����
  - �؂�ւ���FilledRadial
- �Q�[���V�[��(Game)
  - �N���b�N������Q�[���I�[�o�[�V�[����
- �Q�[���I�[�o�[�V�[��(Gameover)
  - �Q�[���V�[���ɃQ�[���I�[�o�[�V�[�����}���`�V�[���ŕ\��
  - �N���b�N������^�C�g���֖߂�
  - �؂�ւ���Fade

## ����

�V�K�Ƀv���W�F�N�g���쐬���čŏ��̐ݒ�����܂��B

1. Unity Hub���N��
1. Unity2021.3.X�ŐV�K�v���W�F�N�g���쐬�B�e���v���[�g�͉��ł��\���܂���
1. �v���W�F�N�g�t�H���_�[����`Packages/manifest.json`���e�L�X�g�G�f�B�^�[�ȂǂŊJ���܂�
   - Visual Studio�Ȃ�ȉ��̑���ŊJ���܂�
     - Project�r���[��Assets���E�N���b�N���� Open C# Project ��I��
     - �\�����[�V�����G�N�X�v���[���[�̃z�[���ׂ̗� �\�����[�V�����Ɨ��p�\�ȃr���[�Ƃ̐؂�ւ� ���N���b�N���� �t�H���_�[�r���[ �ɂ��܂�
     - `Packages/manifest.json`���J���܂�
1. dependencies�̍ŏ��Ɉȉ���ǉ����܂�

```
    "jp.am1.baseframe": "https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1BaseFrame",
    "jp.am1.utils": "https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1Utils",
```

�㏑���ۑ�����Unity�ɐ؂�ւ����玩���I�ɃC���X�g�[�����n�܂�܂��B

�p�b�P�[�W�̃C���X�g�[��������������A�v���W�F�N�g�ŗ��p����A�Z�b�g���C���|�[�g���܂��B

1. Project�E�B���h�E��Assets�t�H���_�[�̉��ɐV�K�t�H���_�[���쐬����`Scripts`�Ƃ������O�ɂ��܂�
1. Tools���j���[���� AM1 > Import BaseFrame Assets ��I�����܂�
1. ��ɍ쐬����`Scripts`�t�H���_�[��I�����܂�
1. �C���|�[�g�_�C�A���O���\�����ꂽ��Import�{�^�����N���b�N���܂�

�r���ŃG���[���\������Ă��C���|�[�g�����������������̂ŋC�ɂ����i�߂Ă��������B

�ȏ�Ńv���W�F�N�g�ɕK�v�Ȃ��̂̒ǉ��͊����ł��B


## �V�[���̍쐬

### �V�X�e���p�V�[���̍쐬
�K�v�ȃV�[�����쐬���܂��B�܂��̓V�X�e���p�̃V�[�������܂��B

1. SampleScene�V�[���̖��O��`System`�ɕύX���āA�V�X�e���p�̃V�[���Ƃ��Ďg���܂�
1. Tools���j���[����AAM1 > Set StateSystem to Active Scene ��I�����āA�ǉ� ���N���b�N���܂�
1. System�V�[���p�̃I�u�W�F�N�g�̐����_�C�A���O���\�����ꂽ��ǉ����N���b�N���܂�
1. TextMeshPro�̃_�C�A���O���\�����ꂽ��Import TMP Essentials���N���b�N���܂�
1. �C���|�[�g������������_�C�A���O����܂�
1. Hierarchy�E�B���h�E����Booter�I�u�W�F�N�g���N���b�N���đI�����܂�
1. Inspector�E�B���h�E��Add Component����Booter�X�N���v�g���A�^�b�`���܂�

�ȏ�ŃV�X�e���V�[���̐ݒ肪�ł��܂����BPlay���ĉ�ʂ��^�����ɂȂ�ΐ����ł��B����͋N���ɔ����ĉ�ʂ𕢂����������s�����܂܂ɂȂ��Ă��邩��ł��B�ŏ��̏�ԂƃV�[�����쐬���Đ؂�ւ��邱�Ƃŏ������J�n�ł��܂��B

���̃V�[�����ŏ��ɋN������悤�ɂ��܂��B���̃V�[���̓}���`�V�[���œǂݍ���ŁA���̃V�[���͔j�����܂���B�풓���������I�u�W�F�N�g��X�N���v�g��z�u���邽�߂̃V�[���Ƃ���System�V�[���𗘗p���Ă��������B

### �e��ԃV�[���̍쐬

�^�C�g���A�Q�[���A�Q�[���I�[�o�[�p�̃V�[�����쐬���܂��B
����m�F���ړI�Ȃ̂ŁATextMeshPro�ŃV�[������\�����������̊ȒP�ȃV�[���ɂ��܂��B

