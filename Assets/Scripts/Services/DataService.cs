using UnityEngine;
using System.Collections.Generic;


public class DataService : MonoBehaviour
{
    [SerializeField] private Sprite[] availableIcons;



    public List<ListItemData> GenerateTestData(int count)
    {
        var data = new List<ListItemData>();
        var ranks = new[] { "Рекрут", "Солдат", "Сержант", "Лейтенант", "Капитан", "Майор", "Полковник", "Генерал" };
        //var levels = new[] { 1, 2, 3, 4, 5 };

        //var random = new Random(12345 % count);

        for (int i = 0; i < count; i++)
        {
            //var rankIndex = i % ranks.Length;
            var rankIndex = Random.Range(0, ranks.Length);
            var rank = ranks[rankIndex];
            //var level = (i / ranks.Length) + 1;
            var level = Random.Range(1, 6);

            var item = new ListItemData(
                title: $"{rank} {level} уровня",
                description: $"Уникальное достижение. Уровень сложности: {(i % 3) + 1}. Особые возможности: +{i * 5} к броне",
                icon: availableIcons[i % availableIcons.Length]
            );

            // Градиент в зависимости от уровня
            var hue = (i * 0.1f) % 1f;
            item.gradientColor1 = Color.HSVToRGB(hue, 0.8f, 1f);
            item.gradientColor2 = Color.HSVToRGB((hue + 0.1f) % 1f, 0.6f, 1f);

            data.Add(item);
        }

        return data;
    }
}