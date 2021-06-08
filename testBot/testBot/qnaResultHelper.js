const { MessageFactory } = require('botbuilder');

/**
 * QnA Maker ����Ԃ��Ă���������K�؂Ȍ`�ɂ��ăN���C�A���g�֕Ԃ�
 * 
 * @param {string} qnaResult QnA Maker ����Ԃ��Ă���answer
 * @returns Activity
 */
module.exports.createAnswer = function(context, qnaResult) {

    if (qnaResult.context && qnaResult.context.prompts && qnaResult.context.prompts.length > 0) {
        // answer��follow-up prompt���t���Ă���
        return createPrompt(qnaResult);
    } else {
        return qnaResult.answer;
    }
}

/**
 * follow-up prompt ���N���C�A���g�֕Ԃ�
 * 
 * @param {QnAMakerResult} qnaResult 
 * @returns Activity
 */
function createPrompt(qnaResult) {
    let title = qnaResult.answer;
    let prompts = qnaResult.context.prompts;
    let buttons = [];

    // DisplayOrder�ŕ��בւ�
    prompts.sort(function(a, b) {
        if (a.displayOrder == b.displayOrder) {
            return 0;
        } else if (a.displayOrder < b.displayOrder) {
            return -1;
        }
        return 1;
    });

    // follow-up prompt�̃{�^�������
    for (let prompt of prompts) {
        buttons.push(prompt.displayText);
    }

    return MessageFactory.suggestedActions(buttons, title);
}// JavaScript source code
