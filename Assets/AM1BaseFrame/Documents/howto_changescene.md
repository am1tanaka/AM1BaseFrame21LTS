# �V�[���̐؂�ւ�

���K�p�̐V�K�v���W�F�N�g������āA�N������V�[���̐؂�ւ����������܂��B�ȉ��̊ȈՂȃQ�[�����[�v���������܂��B

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
����m�F���ړI�Ȃ̂�TextMeshPro�ŃV�[������\�����������̊ȒP�ȃV�[���ɂ��܂��B

## �^�C�g���V�[���̍쐬

�{�V�X�e���̃V�[���؂�ւ��ł́A�ǂݍ��ޑS�ẴV�[����Awake()����������̂�҂����ƁA�؂�ւ����Ǘ�����`ISceneChanger`�����������N���X���K�v�ł��B�����������I�ɍs���c�[�����j���[���g���ăV�[�����쐬���܂��B

1. Tools���j���[���� AM1 > New SceneState ��I�����܂�
1. ��Ԗ��ɍ쐬����V�[��������͂��܂��B�����`Title`�Ƃ��܂�

![�V�[���쐬���j���[](./Images/img00_00.png)

3. �����̃V�[�����쐬���āA�V�[������Title�ɂ������̂ł��̂܂�Create�{�^���������܂�
   - �����ŉ�ʐ؂�ւ��̉��o�ƕb�����ݒ�ł��܂�
3. �ۑ���̃t�H���_�[��`Assets`�ɂ��܂�
   - �I�������t�H���_�[����`Scenes`��`Scripts`�t�H���_�[������΁A���̒��ɍ쐬�����V�[����X�N���v�g��ۑ����܂��B�t�H���_�[�������ꍇ�͑I�������t�H���_�[�ɍ쐬���܂�

�ȉ��̃t�@�C�����쐬����܂��B

 - Scenes/Title�V�[��
 - Scripts/TitleSceneStateChanger�X�N���v�g

Title�V�[���ɂ�`TitleBehaviour`�I�u�W�F�N�g��`AwakeReporter`�X�N���v�g���A�^�b�`����Ĕz�u����Ă��܂��B`AwakeReporter`�X�N���v�g���V�[���̋N����񍐂���X�N���v�g�ł��B���g�̓V���v���Ȃ̂ő��̃V�[���Ǘ��X�N���v�g���쐬������A�������Awake()�ɏ������ړ����Ă��\���܂���B

`TitleSceneStateChanger`�X�N���v�g�́A�V�[���̓ǂݍ��݂⏉�������Ăяo���X�N���v�g���������邽�߂̃X�N���v�g�̂ЂȌ`�ł��B

���Ƃ͈ȉ��̎菇�Ń^�C�g��������\�����܂��B

1. Hierarchy�E�B���h�E��+���N���b�N���āAUI > Canvas ��Title�V�[���ɃL�����o�X���쐬���܂�
1. Canvas��System�V�[���ɍ��ꂽ��A�h���b�O���h���b�v��Title�V�[���Ɉړ����Ă�������
1. EventSystem���h���b�O���āASystem�V�[���ֈړ����܂�
1. �쐬����Canvas��I�����܂�
1. Inspector�E�B���h�E�ŁACanvas Scaler��UI Scale Mode��Scale With Screen Size�ɂ��āA�K����Reference Resolution��ݒ肵�āAScreen Match Mode��Expand�ɂ��܂�
   - ���̍�Ƃ͕K�{�ł͂���܂��񂪁A���C�A�E�g����h�~�ł���Ă����Ƌg
1. �쐬����Canvas���E�N���b�N���āAUI > Text - TextMeshPro ��I�����܂�
1. TITLE�ȂǓK���ɕ�����ݒ肵�ă��C�A�E�g���܂�

![���^�C�g���V�[��](./Images/img00_01.png)

1. �V�[����ۑ����܂�

�ȏ�Ŋ����ł��BHierarchy�E�B���h�E��Title�V�[�����E�N���b�N����Remove Scene�ŉ�����܂��B

���l�̎菇�ŁASphere��u����Game�V�[���AGAME OVER�ƕ\������Gameover�V�[�����쐬���܂��B�V�[�����쐬�������ƂɃX�N���v�g�̃r���h��҂K�v������̂�Create�{�^�����L���ɂȂ�܂ŏ����҂��Ă��������B

![Game�V�[����Gameover�V�[��](./Images/img00_02.png)

## �V�[���̐؂�ւ�������

�V�[���̐؂�ւ��̎������@�ł��B�؂�ւ��ɐ旧���āAFile���j���[����Build Settings���J���āA��ɍ����System, Title, Game, Gameover��4�̃V�[����Scene In Build���ɐݒ肵�܂��BSystem��擪�ɂ��Ă��������B

![�V�[�����r���h�ɐݒ�](./Images/img00_03.png)

### �N������^�C�g����\��

System�V�[�����J����Play����ƁA��ʂ���������ꂽ�܂܂ɂȂ�܂��B�^�C�g���V�[�����N������悤�ɂ��܂��B

�N����V�[����؂�ւ��鎞�̏�����`ISceneStateChanger`�C���^�[�t�F�[�X�����������N���X�ōs���܂��B�N���p�̃X�N���v�g��Scripts�t�H���_�[����`BootSceneStateChanger`�ł��B������J���܂��B

`OnHideScreen()`���\�b�h���ȉ��̂悤�Ɏ�������Ă��܂��B

```cs
    public override void OnHideScreen()
    {
        // �{�����[��������
        new VolumeSetting((int)VolumeType.BGM, new BGMVolumeSaverWithPlayerPrefs());
        BGMSourceAndClips.Instance.SetVolumeSetting(VolumeSetting.volumeSettings[(int)VolumeType.BGM]);
        new VolumeSetting((int)VolumeType.SE, new SEVolumeSaverWithPlayerPrefs());
        SESourceAndClips.Instance.SetVolumeSetting(VolumeSetting.volumeSettings[(int)VolumeType.SE]);
        VolumeSlider.initEvents.Invoke();

        // �x���Đ�������
        SESourceAndClips.Instance.InitDelaySEPlayer(System.Enum.GetValues(typeof(SEPlayer.SE)).Length, SEPlayer.DelaySeconds, SEPlayer.DelayMax);

        // �����ɍŏ��̏�Ԃւ̐؂�ւ��v����ǋL
        // TitleSceneStateChanger.Instance.Request(true);
    }
```

���̃��\�b�h�͉�ʂ����炩�̉��o�ŕ����āA���A�s�v�ɂȂ����V�[���̉��������������Ăяo����܂��B�\�߁ABGM�ƌ��ʉ��̃{�����[���V�X�e����x���Đ��̏������ɉ����āA�Ō�ɃV�[���̐؂�ւ��J�n�����̌Ăяo���Ⴊ�R�����g�A�E�g���ď�����Ă��܂��BTitle�V�[���̖��O��`Title`�ō쐬���Ă���΁A�R�����g�A�E�g����Ă���`TitleSceneStateChanger`�X�N���v�g�������I�ɍ쐬����Ă���̂ł�����Ăяo�����Ƃ��ł��܂��B�ȉ��̂悤�ɃR�����g���O���āA�㏑���ۑ����Ă��������B

```cs
        // �����ɍŏ��̏�Ԃւ̐؂�ւ��v����ǋL
        TitleSceneStateChanger.Instance.Request(true);
```

�㏑���ۑ�������Play���ē�����m�F���Ă��������B

![�^�C�g���V�[���̋N��](./Images/gif00_00.gif)

Title�V�[���̓ǂݍ��݂�����������Filled Radial(��`�̓h��Ԃ�)�̉��o�ŉ�ʂ��\�������悤�ɂȂ�܂����B

### Play���̃V�[���ɂ���
�{�V�X�e���ł́A�r���h���̏������G���[������邽�߂ɋN������ŏ��̃V�[���̐؂�ւ��܂ŃX�N���v�g�Ő��䂵�܂��BUnity�G�f�B�^�[�ő��̃V�[�����J���Ă��Ă��A�����͋N���O�Ɏ����I�ɕ���悤�ɂȂ��Ă��܂��B

## �Q�[���V�[���ւ̐؂�ւ�

�V�[���̐؂�ւ��́A�V�[���쐬���Ɏ����I�ɍ����`????SceneStateChanger`(`????`�̕����̓V�[����)�N���X�̃C���X�^���X�̃��N�G�X�g���Ăяo���܂��B�^�C�g���͎����I�ɋN�������邽�߂ɋN�������ɏ����܂������A�Ăяo���̂͂ǂ�����ł��\���܂���B

�^�C�g���V�[���ő��삪�\�ɂȂ��Ă���A�N���b�N������Q�[�����J�n���鏈�����쐬���܂��B

1. Project�E�B���h�E��Scenes�t�H���_�[����Title�V�[�����h���b�O���āAHierarchy�E�B���h�E�Ƀh���b�v���ă}���`�V�[���ŊJ���܂�
1. Project�E�B���h�E��Scripts�t�H���_�[���E�N���b�N���āACreate > C# Script�ŐV�����X�N���v�g���쐬���܂�
1. �쐬�����X�N���v�g�̖��O��`TitleBehaviour`�ɂ��܂�
1. Project�E�B���h�E����TitleBehaviour�X�N���v�g���h���b�O���āAHierarchy�E�B���h�E��TitleBehaviour�I�u�W�F�N�g�Ƀh���b�v���ăA�^�b�`���܂�
1. Hierarchy�E�B���h�E��TitleBehaviour�X�N���v�g���_�u���N���b�N���ĊJ���܂�

��ʂ̕\�����o���������ăV�[�����J�n�������ǂ����́ASceneStateChanger�N���X��`IsStateStarted()`���\�b�h�Ŋm�F���邱�Ƃ��ł��܂��B

6. TitleBehaviour�X�N���v�g�̍ŏ��̕��Ɉȉ���`using`��ǉ����܂�

```cs
using AM1.BaseFrame;
```

7. Update()���\�b�h���ȉ��̂悤�Ɏ������܂�

```cs
    void Update()
    {
        if (!SceneStateChanger.IsStateStarted(TitleSceneStateChanger.Instance)) return;

        if (Input.GetButtonDown("Fire1"))
        {
            GameSceneStateChanger.Instance.Request();
        }
    }
```

�ŏ���if���Ń^�C�g�����J�n���������m�F���āAfalse�ł܂��؂�ւ����Ȃ�return���ď��������Ȃ��悤�ɂ��Ă��܂��B����if�����v���C���[��G�Ȃǂ̍X�V�X�N���v�g�ɓ����΁A��ʐ؂�ւ����ɓ����Ȃ��悤�ɂł��܂��B

����if����Fire1�Œ�`���Ă���L�[�i�f�t�H���g�ł̓N���b�N��X�y�[�X�L�[�j�������ꂽ�����m�F���āA�Q�[���V�[���֐؂�ւ�����v�����Ăяo���Ă��܂��B

### ����m�F

�ȉ��̓_�ɂ��ē�����m�F���Ă݂Ă��������B

- Play���Ă���N���b�N��A�ł��āA�^�C�g������Q�[���ɐ؂�ւ��^�C�~���O���m�F
- �Q�[���V�[���ւ̐؂�ւ����n�܂��Ă�����N���b�N��A�ł�������

��������V�[���؂�ւ����̕s��|�C���g�ł��B

�����΍�����Ă��Ȃ��ƁA�O�҂̓^�C�g�����\�������O�ɃQ�[�����n�܂��Ă��܂��܂����A��҂̓Q�[���V�[�������x���ǂݒ�������A��R�ǂݍ��񂾂肵�Ă��܂��܂��B���̂悤�ȕs����N���Ȃ��悤�ȑ΍􂪂��Ă���܂��B

### ????SceneStateChanger.Instance.Request()�̈���

�^�C�g�����N�����鎞��`TitleSceneStateChanger.Instance.Request(true);`�A�Q�[���̋N������`GameSceneStateChanger.Instance.Request();`�����s���܂����B�O�҂ɂ͈�����true��n���A��҂͏ȗ����Ă��܂��B

Request()���\�b�h�̈�����true��n���ƁA���łɕʂ̃V�[���؂�ւ��̗v�����o�Ă�����A�V�[�����؂�ւ������������͗v�����L���[�ɐς݂܂��B�O�̃V�[���؂�ւ����I�������A�L���[�ɐς񂾃V�[���ɍX�ɐ؂�ւ���A����Η\��؂�ւ����[�h�ɂȂ�܂��B

�������ȗ����邩false��n���ƁA���łɃV�[���̐؂�ւ��v�����o�Ă�����A�V�[���؂�ւ����̎��́A�V�����V�[���؂�ւ��̗v�����L�����Z�����郂�[�h�ɂȂ�܂��B

�^�C�g���V�[���͋N���������ɃV�[���؂�ւ���\�񂵂����̂ň�����true��n���܂����B�ʏ�́A�Ⴆ�΃Q�[���N���A�Ɠ����t���[���ŃQ�[���I�[�o�[�ɂȂ����ꍇ�Ȃǐ�ɓo�^�����V�[���̐؂�ւ��݂̂�L���ɂ��āA����ȍ~�̓o�^�͖����ɂ��Ȃ��Ɠ��삪���������Ȃ�܂��B���̂��߈����͏ȗ�����̂���{�ł��B





